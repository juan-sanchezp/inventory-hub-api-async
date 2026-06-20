function showSection(name) {
    document.querySelectorAll('.page-section').forEach(el => el.style.display = 'none');
    document.querySelectorAll('.sidebar-icon').forEach(el => el.classList.remove('active'));
    const section = document.getElementById('section-' + name);
    if (section) section.style.display = '';
    const btn = document.querySelector(`.sidebar-icon[data-section="${name}"]`);
    if (btn) btn.classList.add('active');
    if (name === 'customers') loadCustomers();
    if (name === 'sales') { loadCart(); loadSalesHistory(); }
    if (name === 'orders') { }
}

function notify(msg, type) {
    const el = document.createElement('div');
    el.className = 'toast toast-' + (type || 'info');
    el.textContent = msg;
    document.body.appendChild(el);
    setTimeout(() => { el.classList.add('toast-out'); setTimeout(() => el.remove(), 300); }, 2500);
}

function confirmAction(msg) {
    return confirm(msg);
}

function formatCurrency(n) {
    return '$' + Number(n ?? 0).toLocaleString('es-CO', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
}

function formatDate(d) {
    if (!d) return '—';
    const dt = new Date(d);
    return dt.toLocaleDateString('es-CO', { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' });
}

function formatDateShort(d) {
    if (!d) return '—';
    const dt = new Date(d);
    return dt.toLocaleDateString('es-CO', { year: 'numeric', month: '2-digit', day: '2-digit' });
}

function debounce(fn, ms) {
    let timer = null;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => fn.apply(this, args), ms);
    };
}
