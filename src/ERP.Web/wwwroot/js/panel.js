(function () {
  "use strict";

  var token = sessionStorage.getItem("admin_token") || "";
  var state = { page: 1, pageSize: 20, search: "", role: "", status: "" };
  var rolesCache = [];

  var $ = function (id) { return document.getElementById(id); };

  // ---- Labels (API serializa enums como número) ----
  var STATUS_LABELS = { 0: "Pendente", 1: "Ativo", 2: "Inativo", 3: "Suspenso", 4: "Excluído" };
  var STATUS_KEYS = { 0: "pending", 1: "active", 2: "inactive", 3: "suspended", 4: "deleted" };
  var SUB_LABELS = { 0: "Trial", 1: "Ativa", 2: "Pagamento pendente", 3: "Cancelada", 4: "Expirada" };
  var SUB_KEYS = { 0: "trial", 1: "active", 2: "pastdue", 3: "cancelled", 4: "expired" };

  function badge(kind, text) {
    return '<span class="badge badge-' + kind + '">' + esc(text) + "</span>";
  }

  function esc(s) {
    return String(s == null ? "" : s).replace(/[&<>"']/g, function (c) {
      return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c];
    });
  }

  function initials(name) {
    return (name || "?").split(/\s+/).map(function (w) { return w[0]; }).join("").slice(0, 2).toUpperCase();
  }

  function fmtDate(iso) {
    if (!iso) return "—";
    var d = new Date(iso);
    return d.toLocaleDateString("pt-BR") + " " + d.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" });
  }

  // ---- API ----
  function api(path, opts) {
    opts = opts || {};
    opts.headers = Object.assign({ "Content-Type": "application/json" }, opts.headers || {});
    if (token) opts.headers["Authorization"] = "Bearer " + token;
    return fetch(path, opts).then(function (res) {
      return res.json().then(function (body) {
        if (!res.ok || body.success === false) {
          if (res.status === 401) {
            throw Object.assign(new Error("Sessão expirada"), { unauthorized: true });
          }
          throw new Error(body.message || res.statusText);
        }
        return body;
      });
    });
  }

  function handleFailure(err) {
    if (err.unauthorized) {
      sessionStorage.removeItem("admin_token");
      token = "";
      $("panel-view").classList.add("hidden");
      $("login-view").classList.remove("hidden");
      closeModal();
    } else {
      alert("Erro: " + err.message);
    }
  }

  // ---- Login ----
  $("login-form").addEventListener("submit", function (e) {
    e.preventDefault();
    var btn = $("login-btn");
    $("login-error").style.display = "none";
    btn.disabled = true;
    fetch("/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username: $("username").value, password: $("password").value })
    })
      .then(function (res) { return res.json(); })
      .then(function (body) {
        if (!body.success || !body.data || !body.data.token) {
          throw new Error(body.message || "Falha no login");
        }
        token = body.data.token;
        sessionStorage.setItem("admin_token", token);
        showPanel();
      })
      .catch(function (err) {
        var el = $("login-error");
        el.textContent = err.message || "Usuário ou senha inválidos.";
        el.style.display = "block";
      })
      .finally(function () { btn.disabled = false; });
  });

  function showPanel() {
    $("login-view").classList.add("hidden");
    $("panel-view").classList.remove("hidden");
    var me = parseJwt(token);
    var who = me && (me.unique_name || me.nameid);
    if (Array.isArray(who)) who = who[who.length - 1];
    $("logged-user").textContent = who || "administrador";
    loadRoles().then(loadUsers).catch(handleFailure);
  }

  function parseJwt(t) {
    try {
      var payload = t.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
      return JSON.parse(decodeURIComponent(escape(atob(payload))));
    } catch (e) { return null; }
  }

  $("logout-btn").addEventListener("click", function () {
    sessionStorage.removeItem("admin_token");
    token = "";
    $("panel-view").classList.add("hidden");
    $("login-view").classList.remove("hidden");
    $("password").value = "";
  });

  // ---- Papéis disponíveis ----
  function loadRoles() {
    return api("/api/users/roles").then(function (res) {
      rolesCache = res.data || [];
      var filter = $("role-filter");
      while (filter.options.length > 1) filter.remove(1);
      rolesCache.forEach(function (r) {
        var opt = document.createElement("option");
        opt.value = r.name;
        opt.textContent = r.name;
        filter.appendChild(opt);
      });
    });
  }

  function roleOptionsHtml(selectedIds) {
    return rolesCache.map(function (r) {
      var checked = (selectedIds || []).indexOf(r.id) >= 0 ? " checked" : "";
      return '<label class="role-option">' +
        '<input type="checkbox" name="role-checkbox" value="' + r.id + '"' + checked + ">" +
        "<div><div class=\"role-name\">" + esc(r.name) +
        '<span class="role-level">nível ' + r.level + "</span></div>" +
        '<div class="role-desc">' + esc(r.description || "") + "</div></div></label>";
    }).join("");
  }

  function checkedRoleIds() {
    return Array.prototype.slice.call(document.querySelectorAll('input[name="role-checkbox"]:checked'))
      .map(function (cb) { return cb.value; });
  }

  // ---- Listagem ----
  function loadUsers() {
    var q = "?pageNumber=" + state.page + "&pageSize=" + state.pageSize;
    if (state.search) q += "&search=" + encodeURIComponent(state.search);
    if (state.role) q += "&role=" + encodeURIComponent(state.role);
    if (state.status) q += "&status=" + encodeURIComponent(state.status);

    api("/api/users" + q)
      .then(function (res) {
        // PagedResponse vem aninhada: res.data = { pageNumber, totalCount, data: [...] }
        var body = res.data || {};
        var users = body.data || [];
        state.page = body.pageNumber;
        renderRows(users);
        $("page-info").textContent = "Página " + body.pageNumber + " de " + (body.totalPages || 1) + " — " + body.totalCount + " usuário(s)";
        $("prev-page").disabled = !body.hasPreviousPage;
        $("next-page").disabled = !body.hasNextPage;
        loadStats();
      })
      .catch(handleFailure);
  }

  // Estatísticas gerais (sem filtros)
  function loadStats() {
    api("/api/users?pageNumber=1&pageSize=500")
      .then(function (res) {
        var body = res.data || {};
        var users = body.data || [];
        $("stat-total").textContent = body.totalCount;
        $("stat-active").textContent = users.filter(function (u) { return u.status === 1; }).length;
        $("stat-admins").textContent = users.filter(function (u) { return (u.roles || []).indexOf("Admin") >= 0; }).length;
        $("stat-trial").textContent = users.filter(function (u) { return u.tenant && u.tenant.subscriptionStatus === 0; }).length;
      })
      .catch(function () { /* mantém os contadores anteriores */ });
  }

  function renderRows(users) {
    var tbody = $("users-tbody");
    tbody.innerHTML = "";
    $("empty-state").classList.toggle("hidden", users.length > 0);

    users.forEach(function (u) {
      var roleBadges = (u.roles || []).map(function (r) { return badge("role", r); }).join("");
      var rolesCell =
        '<div class="roles-cell">' +
        (roleBadges || '<span class="muted">—</span>') +
        '<button class="icon-btn edit-roles" title="Editar papéis" data-id="' + u.id + '" data-name="' + esc(u.fullName) + '" ' +
        'data-roles=\'' + JSON.stringify(u.roleIds || []) + '\'>✎</button></div>';
      var status = badge(STATUS_KEYS[u.status] || "none", STATUS_LABELS[u.status] || u.status);

      var tenantCell, subCell;
      if (u.tenant) {
        tenantCell = '<div style="font-size:13px;font-weight:600">' + esc(u.tenant.name) + "</div>";
        if (u.tenant.subscriptionStatus != null) {
          subCell = badge(SUB_KEYS[u.tenant.subscriptionStatus] || "none", SUB_LABELS[u.tenant.subscriptionStatus] || u.tenant.subscriptionStatus) +
            '<div class="muted" style="margin-top:4px">' + esc(u.tenant.planName || "Sem plano") +
            (u.tenant.subscriptionEndDate ? " · até " + new Date(u.tenant.subscriptionEndDate).toLocaleDateString("pt-BR") : "") + "</div>";
        } else {
          subCell = badge("none", "Sem assinatura");
        }
      } else {
        tenantCell = '<span class="muted">Global (sem tenant)</span>';
        subCell = badge("none", "—");
      }

      var tr = document.createElement("tr");
      tr.innerHTML =
        '<td><div class="who"><div class="avatar">' + esc(initials(u.fullName)) + '</div><div><div class="name">' +
        esc(u.fullName || u.userName) + '</div><div class="email">' + esc(u.email) + "</div></div></div></td>" +
        "<td>" + rolesCell + "</td>" +
        "<td>" + status + "</td>" +
        "<td>" + tenantCell + "</td>" +
        "<td>" + subCell + "</td>" +
        '<td class="muted">' + fmtDate(u.lastLogin) + "</td>";
      tbody.appendChild(tr);
    });

    Array.prototype.forEach.call(document.querySelectorAll(".edit-roles"), function (btn) {
      btn.addEventListener("click", function () {
        openEditRolesModal(btn.dataset.id, btn.dataset.name, JSON.parse(btn.dataset.roles));
      });
    });
  }

  // ---- Modal genérico ----
  function closeModal() {
    var overlay = document.querySelector(".modal-overlay");
    if (overlay) overlay.remove();
  }

  document.addEventListener("keydown", function (e) {
    if (e.key === "Escape") closeModal();
  });

  function openModal(title, bodyHtml, onSubmit, submitLabel) {
    closeModal();
    var overlay = document.createElement("div");
    overlay.className = "modal-overlay";
    overlay.innerHTML =
      '<div class="modal"><h2>' + esc(title) + '</h2>' +
      '<div class="form-error" id="modal-error"></div>' +
      bodyHtml +
      '<div class="actions">' +
      '<button class="btn secondary" id="modal-cancel">Cancelar</button>' +
      '<button class="btn" id="modal-submit">' + esc(submitLabel || "Salvar") + "</button></div></div>";
    document.body.appendChild(overlay);
    overlay.addEventListener("click", function (e) { if (e.target === overlay) closeModal(); });
    $("modal-cancel").addEventListener("click", closeModal);
    $("modal-submit").addEventListener("click", function () {
      $("modal-error").style.display = "none";
      onSubmit();
    });
    var firstInput = overlay.querySelector("input, [tabindex]");
    if (firstInput) firstInput.focus();
  }

  function modalError(msg) {
    var el = $("modal-error");
    el.textContent = msg;
    el.style.display = "block";
  }

  // ---- Modal: editar papéis do usuário ----
  function openEditRolesModal(userId, userName, currentRoleIds) {
    openModal(
      "Papéis de " + (userName || "usuário"),
      roleOptionsHtml(currentRoleIds) + '<p class="modal-sub">Marque os papéis que este usuário deve ter.</p>',
      function () {
        var ids = checkedRoleIds();
        if (ids.length === 0) { modalError("O usuário deve ter pelo menos um papel."); return; }
        $("modal-submit").disabled = true;
        api("/api/users/" + userId + "/roles", {
          method: "PUT",
          body: JSON.stringify({ roleIds: ids })
        })
          .then(function () { closeModal(); loadUsers(); })
          .catch(function (err) {
            $("modal-submit").disabled = false;
            err.unauthorized ? handleFailure(err) : modalError(err.message);
          });
      },
      "Salvar papéis"
    );
  }

  // ---- Modal: novo usuário ----
  $("new-user-btn").addEventListener("click", function () {
    openModal(
      "Novo usuário",
      '<div class="field"><label>Nome</label><input id="nu-first-name"></div>' +
      '<div class="field"><label>Sobrenome</label><input id="nu-last-name"></div>' +
      '<div class="field"><label>Usuário</label><input id="nu-username"></div>' +
      '<div class="field"><label>E-mail</label><input id="nu-email" type="email"></div>' +
      '<div class="field"><label>Senha inicial</label><input id="nu-password" type="password"></div>' +
      '<p class="modal-sub" style="margin:14px 0 8px">Papéis (opcional — sem seleção, recebe o papel básico "User"):</p>' +
      roleOptionsHtml([]),
      function () {
        var payload = {
          firstName: $("nu-first-name").value.trim(),
          lastName: $("nu-last-name").value.trim(),
          userName: $("nu-username").value.trim(),
          email: $("nu-email").value.trim(),
          password: $("nu-password").value,
          roleIds: checkedRoleIds()
        };
        if (!payload.firstName || !payload.userName || !payload.email || !payload.password) {
          modalError("Preencha nome, usuário, e-mail e senha.");
          return;
        }
        $("modal-submit").disabled = true;
        api("/api/users", { method: "POST", body: JSON.stringify(payload) })
          .then(function () { closeModal(); loadUsers(); })
          .catch(function (err) {
            $("modal-submit").disabled = false;
            err.unauthorized ? handleFailure(err) : modalError(err.message);
          });
      },
      "Criar usuário"
    );
  });

  // ---- Filtros e paginação ----
  var searchTimer;
  $("search").addEventListener("input", function () {
    clearTimeout(searchTimer);
    searchTimer = setTimeout(function () {
      state.search = $("search").value.trim();
      state.page = 1;
      loadUsers();
    }, 300);
  });
  $("role-filter").addEventListener("change", function () {
    state.role = $("role-filter").value;
    state.page = 1;
    loadUsers();
  });
  $("status-filter").addEventListener("change", function () {
    state.status = $("status-filter").value;
    state.page = 1;
    loadUsers();
  });
  $("prev-page").addEventListener("click", function () { state.page--; loadUsers(); });
  $("next-page").addEventListener("click", function () { state.page++; loadUsers(); });

  // Sessão salva: entra direto no painel
  if (token) showPanel();
})();
