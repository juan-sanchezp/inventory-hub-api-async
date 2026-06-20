let currentCustomerId = null;
let customersCache = [];

async function loadCustomers(search) {
    try {
        const res = await apiGet(`${API_BASE}/api/customers`);
        const data = extractData(res) || [];
        customersCache = data;
        renderCustomers(data, search);
    } catch (e) {
        console.error('Error loading customers:', e);
        renderCustomers([], search);
    }
}

function renderCustomers(customers, search) {
    const tbody = document.getElementById('customer-table');
    const empty = document.getElementById('customer-empty');
    tbody.innerHTML = '';

    let filtered = customers;
    if (search) {
        const q = search.toLowerCase();
        filtered = customers.filter(c =>
            c.name.toLowerCase().includes(q) ||
            c.documentNumber.toLowerCase().includes(q) ||
            (c.phoneNumber && c.phoneNumber.includes(q)) ||
            (c.email && c.email.toLowerCase().includes(q))
        );
    }

    document.getElementById('customer-count').textContent = filtered.length;
    document.getElementById('customer-active-count').textContent = filtered.filter(c => c.isActive !== false).length;

    if (!filtered.length) {
        empty.style.display = '';
        return;
    }
    empty.style.display = 'none';

    const frag = document.createDocumentFragment();
    filtered.forEach(c => {
        const tr = document.createElement('tr');
        const active = c.isActive !== false;
        tr.innerHTML = `
            <td><span class="cust-doc">${esc(c.documentType || 'CC')} · ${esc(c.documentNumber)}</span></td>
            <td class="td-name"><span class="cust-dot ${active ? 'dot-active' : 'dot-inactive'}"></span> 👤 ${esc(c.name)}</td>
            <td>📞 ${esc(c.phoneNumber || '—')}</td>
            <td>${esc(c.email || '—')}</td>
            <td>📍 ${esc(c.city || '—')}</td>
            <td>
                <span class="btn-icon-wrap"><button class="btn-icon" onclick="editCustomer(${c.id})" title="Editar">✏️</button></span>
                <span class="btn-icon-wrap"><button class="btn-icon" onclick="deleteCustomer(${c.id})" title="Eliminar">🗑️</button></span>
            </td>
        `;
        frag.appendChild(tr);
    });
    tbody.appendChild(frag);
}

function filterCustomers() {
    const q = document.getElementById('customer-search').value.trim();
    loadCustomers(q);
}

const filterCustomersDebounced = debounce(filterCustomers, 300);

function openCustomerModal(customer) {
    const modal = document.getElementById('customer-modal');
    const title = document.getElementById('customer-modal-title');

    if (customer) {
        title.textContent = 'Editar cliente';
        document.getElementById('cf-name').value = customer.name || '';
        document.getElementById('cf-documentType').value = customer.documentType || 'CC';
        document.getElementById('cf-documentNumber').value = customer.documentNumber || '';
        document.getElementById('cf-phone').value = customer.phoneNumber || '';
        document.getElementById('cf-email').value = customer.email || '';
        document.getElementById('cf-address').value = customer.address || '';
        document.getElementById('cf-city').value = customer.city || '';
        document.getElementById('cf-notes').value = customer.notes || '';
        document.getElementById('cf-isActive').checked = customer.isActive !== false;
        currentCustomerId = customer.id;
    } else {
        title.textContent = 'Nuevo cliente';
        document.getElementById('cf-name').value = '';
        document.getElementById('cf-documentType').value = 'CC';
        document.getElementById('cf-documentNumber').value = '';
        document.getElementById('cf-phone').value = '';
        document.getElementById('cf-email').value = '';
        document.getElementById('cf-address').value = '';
        document.getElementById('cf-city').value = '';
        document.getElementById('cf-notes').value = '';
        document.getElementById('cf-isActive').checked = true;
        currentCustomerId = null;
    }
    modal.style.display = 'flex';
    modal.classList.add('open');
    document.body.style.overflow = 'hidden';
}

function closeCustomerModal() {
    const modal = document.getElementById('customer-modal');
    modal.style.display = 'none';
    modal.classList.remove('open');
    document.body.style.overflow = '';
    currentCustomerId = null;
}

function handleCustomerModalBg(e) {
    if (e.target === document.getElementById('customer-modal')) closeCustomerModal();
}

async function saveCustomer() {
    const data = {
        name: document.getElementById('cf-name').value.trim(),
        documentNumber: document.getElementById('cf-documentNumber').value.trim(),
        documentType: document.getElementById('cf-documentType').value.trim() || 'CC',
        phoneNumber: document.getElementById('cf-phone').value.trim() || '',
        email: document.getElementById('cf-email').value.trim() || '',
        address: document.getElementById('cf-address').value.trim() || '',
        city: document.getElementById('cf-city').value.trim() || '',
        notes: document.getElementById('cf-notes').value.trim() || '',
        isActive: document.getElementById('cf-isActive').checked
    };

    if (!data.name) { notify('El nombre es requerido', 'error'); return; }
    if (!data.documentNumber) { notify('El número de documento es requerido', 'error'); return; }

    try {
        if (currentCustomerId) {
            await apiPut(`${API_BASE}/api/customers/${currentCustomerId}`, data);
            notify('Cliente actualizado exitosamente', 'success');
        } else {
            await apiPost(`${API_BASE}/api/customers`, data);
            notify('Cliente creado exitosamente', 'success');
        }
        closeCustomerModal();
        loadCustomers();
    } catch (e) {
        notify('Error: ' + e.message, 'error');
    }
}

async function editCustomer(id) {
    try {
        const res = await apiGet(`${API_BASE}/api/customers/${id}`);
        const customer = extractData(res);
        if (customer) openCustomerModal(customer);
    } catch (e) {
        notify('Error al cargar cliente: ' + e.message, 'error');
    }
}

async function deleteCustomer(id) {
    if (!confirmAction('¿Eliminar este cliente?')) return;
    try {
        await apiDelete(`${API_BASE}/api/customers/${id}`);
        notify('Cliente eliminado exitosamente', 'success');
        loadCustomers();
    } catch (e) {
        notify('Error: ' + e.message, 'error');
    }
}
