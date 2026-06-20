let currentCart = null;
let saleProductsCache = [];

// ==================== CART ====================

async function loadCart() {
    try {
        const [cartRes] = await Promise.all([
            apiGet(`${API_BASE}/api/sale/cart`),
            loadSaleProducts()
        ]);
        currentCart = extractData(cartRes);
        renderCart(currentCart);
        populateCustomerSelect();
    } catch (e) {
        console.error('Error loading cart:', e);
        renderCart(null);
    }
}

function renderCart(cart) {
    const tbody = document.getElementById('sale-cart-table');
    const empty = document.getElementById('sale-cart-empty');
    tbody.innerHTML = '';

    if (!cart || !cart.details || !cart.details.length) {
        empty.style.display = '';
        document.getElementById('sale-subtotal').textContent = '$0';
        document.getElementById('sale-tax').textContent = '$0';
        document.getElementById('sale-total').textContent = '$0';
        document.getElementById('sale-cart-id').textContent = '—';
        document.getElementById('sale-checkout-btn').disabled = true;
        updateCartFabBadge();
        return;
    }
    empty.style.display = 'none';
    document.getElementById('sale-cart-id').textContent = cart.id || '—';
    document.getElementById('sale-checkout-btn').disabled = false;

    const frag = document.createDocumentFragment();
    cart.details.forEach(d => {
        const tr = document.createElement('tr');
        const product = saleProductsCache.find(p => p.id === d.productId);
        const code = product ? esc(product.code) : '';
        tr.innerHTML = `
            <td class="td-code-sm">${code}</td>
            <td class="td-name">${esc(d.productName)}</td>
            <td>
                <div class="cart-qty-wrap">
                    <button class="cart-qty-btn" onclick="updateCartItemQty(${d.id}, ${d.quantity - 1})">−</button>
                    <span class="cart-qty-val" id="qty-${d.id}">${d.quantity}</span>
                    <button class="cart-qty-btn" onclick="updateCartItemQty(${d.id}, ${d.quantity + 1})">+</button>
                </div>
            </td>
            <td class="td-price">${formatCurrency(d.unitPrice)}</td>
            <td class="td-price">${formatCurrency(d.subTotal)}</td>
            <td>
                <button class="btn-ver" style="color:var(--red);border-color:rgba(239,68,68,0.3)" onclick="removeFromCart(${d.id})">✕</button>
            </td>
        `;
        frag.appendChild(tr);
    });
    tbody.appendChild(frag);

    document.getElementById('sale-subtotal').textContent = formatCurrency(cart.subTotal);
    document.getElementById('sale-tax').textContent = formatCurrency(cart.tax);
    document.getElementById('sale-total').textContent = formatCurrency(cart.total);

    updateCartFabBadge();
}

let customerList = [];

async function populateCustomerSelect() {
    try {
        const res = await apiGet(`${API_BASE}/api/customers`);
        customerList = extractData(res) || [];
        if (currentCart?.customerId) {
            const c = customerList.find(x => x.id === currentCart.customerId);
            if (c) {
                document.getElementById('sale-customer-input').value = `${c.documentType || 'CC'} ${c.documentNumber} - ${c.name}`;
                document.getElementById('sale-customer-id').value = c.id;
            }
        }
    } catch (e) {
        console.error('Error loading customers:', e);
    }
}

function filterCustomerDropdown() {
    const input = document.getElementById('sale-customer-input');
    const dropdown = document.getElementById('sale-customer-dropdown');
    const query = input.value.toLowerCase().trim();

    if (!query) {
        dropdown.style.display = 'none';
        document.getElementById('sale-customer-id').value = '';
        return;
    }

    const filtered = customerList.filter(c =>
        c.name.toLowerCase().includes(query) ||
        c.documentNumber.toLowerCase().includes(query) ||
        (c.phoneNumber && c.phoneNumber.includes(query))
    );

    if (!filtered.length) {
        dropdown.style.display = 'none';
        return;
    }

    dropdown.innerHTML = filtered.map(c =>
        `<div onclick="selectCustomer(${c.id}, '${escAttr(c.documentType || 'CC')} ${escAttr(c.documentNumber)} - ${escAttr(c.name)}')"
              style="padding:12px 16px;cursor:pointer;border-bottom:1px solid var(--border);font-size:17px;color:var(--text);transition:background 0.1s"
              onmouseover="this.style.background='var(--surface2)'"
              onmouseout="this.style.background=''">
            <div style="font-weight:700;font-size:17px">${esc(c.name)}</div>
            <div style="font-size:14px;color:var(--muted)">${c.documentType || 'CC'} ${c.documentNumber}${c.phoneNumber ? ' — ' + c.phoneNumber : ''}</div>
        </div>`
    ).join('');
    dropdown.style.display = '';
}

function selectCustomer(id, label) {
    document.getElementById('sale-customer-input').value = label;
    document.getElementById('sale-customer-id').value = id;
    document.getElementById('sale-customer-dropdown').style.display = 'none';
}

document.addEventListener('click', function(e) {
    const container = document.getElementById('sale-customer-input')?.closest('.drawer-section');
    if (container && !container.contains(e.target)) {
        document.getElementById('sale-customer-dropdown').style.display = 'none';
    }
});

function escAttr(s) {
    return s.replace(/&/g,'&amp;').replace(/"/g,'&quot;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
}

async function loadSaleProducts() {
    try {
        const res = await apiGet(`${API_BASE}/api/products`);
        saleProductsCache = extractData(res) || [];
        return saleProductsCache;
    } catch (e) {
        console.error('Error loading products for sale:', e);
        return [];
    }
}

function searchSaleProducts() {
    const q = document.getElementById('sale-product-search').value.trim().toLowerCase();
    const results = document.getElementById('sale-product-results');
    results.innerHTML = '';

    let filtered = saleProductsCache;
    if (q) {
        filtered = saleProductsCache.filter(p =>
            p.name.toLowerCase().includes(q) ||
            p.code.toLowerCase().includes(q) ||
            p.barcode?.toLowerCase().includes(q)
        );
    }
    if (!filtered.length) {
        results.innerHTML = '<div style="padding:12px;color:var(--muted);text-align:center">Sin resultados</div>';
        return;
    }
    const frag = document.createDocumentFragment();
    filtered.slice(0, 10).forEach(p => {
        const div = document.createElement('div');
        div.className = 'sale-product-item';
        div.innerHTML = `
            <div style="flex:1">
                <strong>${esc(p.code)}</strong> — ${esc(p.name)}
                <div style="font-size:12px;color:var(--muted)">Stock: ${p.stock} · ${formatCurrency(p.price)}</div>
            </div>
            <button class="btn-ver" onclick="addToCart(${p.id})">Agregar</button>
        `;
        frag.appendChild(div);
    });
    results.appendChild(frag);
}

async function addToCart(productId) {
    const qty = 1;
    try {
        const res = await apiPost(`${API_BASE}/api/sale/cart/add`, { productId, quantity: qty });
        currentCart = extractData(res);
        renderCart(currentCart);

        const product = saleProductsCache.find(p => p.id === productId);
        const name = product ? product.name : 'Producto';
        notify(`${name} agregado 🛒`, 'success');

        const fabBadge = document.getElementById('cart-fab-badge');
        if (fabBadge) {
            fabBadge.style.transform = 'scale(1.5)';
            fabBadge.style.transition = 'transform 0.15s';
            setTimeout(() => { fabBadge.style.transform = 'scale(1)'; }, 200);
        }

        const searchEl = document.getElementById('sale-product-search');
        if (searchEl) searchEl.value = '';
        const resultsEl = document.getElementById('sale-product-results');
        if (resultsEl) resultsEl.innerHTML = '';
    } catch (e) {
        notify('Error al agregar: ' + e.message, 'error');
    }
}

async function updateCartItemQty(detailId, qty) {
    if (qty < 1) {
        removeFromCart(detailId);
        return;
    }
    try {
        const res = await apiPut(`${API_BASE}/api/sale/cart/item/${detailId}`, { quantity: qty });
        currentCart = extractData(res);
        renderCart(currentCart);
    } catch (e) {
        notify('Error al actualizar: ' + e.message, 'error');
    }
}

async function removeFromCart(detailId) {
    if (!confirmAction('¿Quitar este producto del carrito?')) return;
    try {
        const res = await apiDelete(`${API_BASE}/api/sale/cart/item/${detailId}`);
        currentCart = extractData(res);
        renderCart(currentCart);
        notify('Producto eliminado del carrito', 'success');
    } catch (e) {
        notify('Error al eliminar: ' + e.message, 'error');
    }
}

async function clearCart() {
    if (!confirmAction('¿Vaciar todo el carrito?')) return;
    try {
        const res = await apiDelete(`${API_BASE}/api/sale/cart/clear`);
        currentCart = extractData(res);
        renderCart(currentCart);
        notify('Carrito vaciado', 'success');
    } catch (e) {
        notify('Error: ' + e.message, 'error');
    }
}

async function checkoutSale() {
    if (!currentCart || !currentCart.id) return;
    const customerId = document.getElementById('sale-customer-id').value;
    const documentType = parseInt(document.getElementById('sale-document-type').value) || 1;
    const paymentMethod = document.querySelector('input[name="payment-method"]:checked')?.value || 'Efectivo';

    if (!customerId) {
        notify('Debe seleccionar un cliente para realizar la venta', 'error');
        document.getElementById('sale-customer-input').focus();
        return;
    }

    if (!confirmAction('¿Finalizar esta venta?')) return;

    const soldProductIds = currentCart?.details?.map(d => d.productId).filter(Boolean) || [];

    try {
        const payload = {
            customerId: customerId ? parseInt(customerId) : null,
            documentType: documentType,
            paymentMethod: paymentMethod,
            dueDate: null
        };
        const res = await apiPost(`${API_BASE}/api/sale/${currentCart.id}/checkout`, payload);
        notify('Venta finalizada exitosamente', 'success');
        currentCart = null;
        renderCart(null);
        loadSalesHistory();
        populateCustomerSelect();
        document.getElementById('sale-customer-input').value = '';
        document.getElementById('sale-customer-id').value = '';
        closeCartDrawer();
        if (typeof refreshProductsByIds === 'function') refreshProductsByIds(soldProductIds);
    } catch (e) {
        notify('Error al finalizar venta: ' + e.message, 'error');
    }
}

// ==================== SALES HISTORY ====================

async function loadSalesHistory() {
    try {
        const productCode = document.getElementById('filter-sale-product-code')?.value?.trim();
        const customerName = document.getElementById('filter-sale-customer')?.value?.trim();
        const startDate = document.getElementById('filter-sale-date-from')?.value;
        const endDate = document.getElementById('filter-sale-date-to')?.value;

        const params = new URLSearchParams();
        if (productCode) params.append('productCode', productCode);
        if (customerName) params.append('customerName', customerName);
        if (startDate) params.append('startDate', startDate);
        if (endDate) params.append('endDate', endDate);

        const qs = params.toString();
        const url = `${API_BASE}/api/sale${qs ? '?' + qs : ''}`;

        const res = await apiGet(url);
        const sales = extractData(res) || [];
        renderSalesHistory(sales);
    } catch (e) {
        console.error('Error loading sales:', e);
        renderSalesHistory([]);
    }
}

function applySaleFilters() {
    loadSalesHistory();
}

function clearSaleFilters() {
    document.getElementById('filter-sale-product-code').value = '';
    document.getElementById('filter-sale-customer').value = '';
    document.getElementById('filter-sale-date-from').value = '';
    document.getElementById('filter-sale-date-to').value = '';
    loadSalesHistory();
}

function renderSalesHistory(sales) {
    const tbody = document.getElementById('sale-history-table');
    const empty = document.getElementById('sale-history-empty');
    tbody.innerHTML = '';

    const statusMap = { 1: 'Borrador', 2: 'Confirmada', 3: 'Completada', 4: 'Cancelada' };
    const statusClass = { 1: 'sh-badge-gray', 2: 'sh-badge-orange', 3: 'sh-badge-green', 4: 'sh-badge-red' };
    const docTypeMap = { 1: 'POS', 2: 'Electrónica' };
    const paymentStatusMap = { 1: 'Pendiente', 2: 'Parcial', 3: 'Pagada', 4: 'Vencida', 5: 'Anulada' };
    const paymentColor = { 1: '#ef4444', 2: '#d97706', 3: '#16a34a', 4: '#dc2626', 5: '#6b7280' };

    const completed = sales.filter(s => s.status >= 2);
    document.getElementById('sale-history-count').textContent = completed.length + ' venta' + (completed.length !== 1 ? 's' : '');

    if (!sales.length) {
        empty.style.display = '';
        return;
    }
    empty.style.display = 'none';

    const frag = document.createDocumentFragment();
    const claimLabelMap = {
        'Cambio mismo': 'CAMBIO',
        'Cambio otro': 'CAMBIO',
        'Reparado': 'REPARADO',
        'Reembolsado': 'DEVOLUCIÓN'
    };
    sales.forEach(s => {
        const tr = document.createElement('tr');
        const claims = s.details ? s.details.filter(d => d.warrantyClaimed) : [];
        let claimBadge = '';
        if (claims.length) {
            const label = claimLabelMap[claims[0].warrantyResolution] || 'GARANTÍA';
            claimBadge = `<span class="sh-claim-badge">🔧 ${label}</span>`;
        }
        tr.innerHTML = `
            <td><span class="sh-id">#${s.id}</span></td>
            <td class="sh-customer">${esc(s.customerName || '—')}</td>
            <td class="sh-date">${formatDate(s.saleDate)}</td>
            <td class="sh-total">${formatCurrency(s.total)}</td>
            <td><span class="sh-badge ${statusClass[s.status] || 'sh-badge-green'}">${statusMap[s.status] || s.status}</span></td>
            <td><span class="sh-payment" style="color:${paymentColor[s.paymentStatus] || 'var(--text)'}">${paymentStatusMap[s.paymentStatus] || '—'} ${claimBadge}</span></td>
            <td><span class="sh-type">${docTypeMap[s.documentType] || '—'}</span></td>
            <td><button class="sh-view-btn" onclick="viewSaleDetail(${s.id})">Ver</button></td>
        `;
        frag.appendChild(tr);
    });
    tbody.appendChild(frag);
}

async function viewSaleDetail(saleId) {
    try {
        const res = await apiGet(`${API_BASE}/api/sale/${saleId}`);
        const sale = extractData(res);
        if (!sale) { notify('Venta no encontrada', 'error'); return; }

        currentSaleForPayment = sale;
        renderPaymentSummary(sale);

        const modal = document.getElementById('sale-detail-modal');
        const statusMap = { 1: 'Borrador', 2: 'Confirmada', 3: 'Completada', 4: 'Cancelada' };
        const docTypeMap = { 1: 'POS', 2: 'Electrónica' };
        const paymentStatusMap = { 1: 'Pendiente', 2: 'Pago Parcial', 3: 'Pagada', 4: 'Vencida', 5: 'Anulada' };

        document.getElementById('sd-id').textContent = '#' + sale.id;

        // Info items con badges de estado
        document.getElementById('sd-customer').textContent = sale.customerName || '—';
        document.getElementById('sd-date').textContent = formatDate(sale.saleDate);

        const statusEl = document.getElementById('sd-status');
        const statusVal = statusMap[sale.status] || sale.status;
        statusEl.textContent = statusVal;
        const badgeClass = 'sd-badge-' + (sale.status === 3 ? 'completed' : sale.status === 1 ? 'draft' : 'cancelled');
        statusEl.classList.remove('sd-badge-completed', 'sd-badge-draft', 'sd-badge-cancelled');
        statusEl.classList.add(badgeClass);

        const paymentStatusEl = document.getElementById('sd-payment-status');
        const payVal = paymentStatusMap[sale.paymentStatus] || '—';
        paymentStatusEl.textContent = payVal;
        const badgeClass2 = 'sd-badge-' + (sale.paymentStatus === 3 ? 'paid' : sale.paymentStatus === 2 ? 'partial' : sale.paymentStatus === 4 ? 'overdue' : 'pending');
        paymentStatusEl.classList.remove('sd-badge-paid', 'sd-badge-partial', 'sd-badge-pending', 'sd-badge-overdue');
        paymentStatusEl.classList.add(badgeClass2);

        document.getElementById('sd-doc-type-badge').textContent = docTypeMap[sale.documentType] || sale.documentType;
        document.getElementById('sd-subtotal').textContent = formatCurrency(sale.subTotal);
        document.getElementById('sd-tax').textContent = formatCurrency(sale.tax);
        document.getElementById('sd-total').textContent = formatCurrency(sale.total);

        // Renderizar productos como filas de tabla con estilo card
        const tbody = document.getElementById('sd-details-tbody');
        tbody.innerHTML = '';
        if (sale.details && sale.details.length) {
            sale.details.forEach((d, i) => {
                const tr = document.createElement('tr');

                const warrantyText = d.warrantyEndDate
                    ? `${d.warrantyDays}d — Hasta ${formatDateShort(d.warrantyEndDate)}`
                    : '—';
                const inWarranty = d.warrantyEndDate && new Date(d.warrantyEndDate) > new Date() && !d.warrantyClaimed;
                const claimed = d.warrantyClaimed;

                let warrantyHtml = `<span class="sd-warr-text">${warrantyText}</span>`;
                if (inWarranty) {
                    warrantyHtml += `<button class="btn-warranty" onclick="claimWarranty(${d.id}, event)">🔧</button>`;
                } else if (claimed) {
                    let info = `Reclamado ${formatDate(d.warrantyClaimDate)} — ${esc(d.warrantyResolution || '')}`;
                    if (d.warrantyReplacementCode) info += ` → ${esc(d.warrantyReplacementCode)}`;
                    warrantyHtml += `<span class="sd-warr-claimed">${info}</span>`;
                }

                tr.innerHTML = `
                    <td><span class="sd-td-code">${esc(d.productCode || '—')}</span></td>
                    <td class="sd-td-name">${esc(d.productName)} <span class="sd-td-unit">${formatCurrency(d.unitPrice)} c/u</span></td>
                    <td class="sd-td-qty">${d.quantity}</td>
                    <td class="sd-td-total">${formatCurrency(d.subTotal)}</td>
                    <td class="sd-td-warranty">${warrantyHtml}</td>
                `;
                tbody.appendChild(tr);
            });
        }

        modal.style.display = 'flex';
        modal.classList.add('open');
        document.body.style.overflow = 'hidden';
    } catch (e) {
        notify('Error al cargar detalle: ' + e.message, 'error');
    }
}

function closeSaleDetailModal() {
    const modal = document.getElementById('sale-detail-modal');
    modal.style.display = 'none';
    modal.classList.remove('open');
    document.body.style.overflow = '';
}

function handleSaleDetailBg(e) {
    if (e.target === document.getElementById('sale-detail-modal')) closeSaleDetailModal();
}

// ==================== CART DRAWER ====================

function openCartDrawer() {
    loadCart();
    const overlay = document.getElementById('cart-drawer-overlay');
    const drawer = document.getElementById('cart-drawer');
    overlay.classList.add('open');
    drawer.classList.add('open');
    document.body.style.overflow = 'hidden';
}

function closeCartDrawer() {
    const overlay = document.getElementById('cart-drawer-overlay');
    const drawer = document.getElementById('cart-drawer');
    overlay.classList.remove('open');
    drawer.classList.remove('open');
    document.body.style.overflow = '';
}

function updateCartFabBadge() {
    const fabBadge = document.getElementById('cart-fab-badge');
    const fab = document.getElementById('cart-fab');
    const drawerBadge = document.getElementById('cart-drawer-badge');
    const count = currentCart?.details?.reduce((s, d) => s + (d.quantity || 0), 0) || 0;

    if (fab) {
        fab.classList.toggle('has-items', count > 0);
    }

    if (fabBadge) {
        if (count > 0) {
            const prev = fabBadge.textContent;
            fabBadge.textContent = count;
            fabBadge.style.display = '';
            if (prev !== String(count)) {
                fabBadge.classList.remove('pop');
                void fabBadge.offsetWidth;
                fabBadge.classList.add('pop');
            }
        } else {
            fabBadge.style.display = 'none';
        }
    }

    if (drawerBadge) {
        if (count > 0) {
            drawerBadge.textContent = count;
            drawerBadge.style.display = '';
        } else {
            drawerBadge.style.display = 'none';
        }
    }
}

// ==================== PAYMENTS ====================

let currentSaleForPayment = null;

function openPaymentModal() {
    if (!currentSaleForPayment) return;
    document.getElementById('pm-sale-id').textContent = '#' + currentSaleForPayment.id;
    document.getElementById('pm-total').textContent = formatCurrency(currentSaleForPayment.total);
    document.getElementById('pm-amount').value = '';
    document.getElementById('pm-amount').max = currentSaleForPayment.total;
    document.getElementById('pm-reference').value = '';
    document.getElementById('pm-notes').value = '';
    document.getElementById('payment-modal').style.display = 'flex';
    document.getElementById('payment-modal').classList.add('open');
}

function closePaymentModal() {
    document.getElementById('payment-modal').style.display = 'none';
    document.getElementById('payment-modal').classList.remove('open');
}

async function recordPayment() {
    if (!currentSaleForPayment) return;
    const amount = parseFloat(document.getElementById('pm-amount').value);
    if (!amount || amount <= 0) { notify('Ingrese un monto válido', 'error'); return; }

    const payload = {
        amount: amount,
        paymentMethod: document.getElementById('pm-method').value,
        reference: document.getElementById('pm-reference').value || null,
        notes: document.getElementById('pm-notes').value || null
    };

    try {
        const res = await apiPost(`${API_BASE}/api/payment/sale/${currentSaleForPayment.id}`, payload);
        notify('Abono registrado exitosamente', 'success');
        closePaymentModal();
        viewSaleDetail(currentSaleForPayment.id);
    } catch (e) {
        notify('Error al registrar abono: ' + e.message, 'error');
    }
}

function renderPaymentSummary(sale) {
    const container = document.getElementById('sd-payment-summary');
    const payBtn = document.getElementById('sd-pay-btn');

    if (!sale || sale.status < 2) {
        container.innerHTML = '';
        payBtn.style.display = 'none';
        return;
    }

    const showPay = sale.paymentStatus === 1 || sale.paymentStatus === 2;
    payBtn.style.display = showPay ? '' : 'none';
    currentSaleForPayment = sale;

    if (sale.paymentStatus === 3) {
        container.innerHTML = `<div style="padding:10px 14px;background:#dcfce7;border-radius:8px;font-size:13px;color:#166534">✅ Pagada</div>`;
        return;
    }

    apiGet(`${API_BASE}/api/payment/sale/${sale.id}`).then(res => {
        const payments = extractData(res);
        if (!payments || !payments.length) {
            container.innerHTML = `<div style="padding:10px 14px;background:#fef3c7;border-radius:8px;font-size:13px;color:#92400e">⏳ Pendiente de pago — Total: ${formatCurrency(sale.total)}</div>`;
            return;
        }
        const totalPaid = payments.reduce((s, p) => s + p.amount, 0);
        const remaining = sale.total - totalPaid;
        let html = `<div style="background:var(--surface2);border-radius:8px;padding:12px;font-size:13px">`;
        html += `<div style="font-weight:600;margin-bottom:8px">💰 Historial de abonos</div>`;
        payments.forEach(p => {
            html += `<div style="display:flex;justify-content:space-between;padding:4px 0;border-bottom:1px solid var(--border)">
                <span>${formatDate(p.paymentDate)} — ${p.paymentMethod}${p.reference ? ' (' + esc(p.reference) + ')' : ''}</span>
                <span style="font-weight:600;color:var(--green)">${formatCurrency(p.amount)}</span>
            </div>`;
        });
        html += `<div style="display:flex;justify-content:space-between;padding:6px 0;font-weight:600">
            <span>Total abonado</span><span style="color:var(--green)">${formatCurrency(totalPaid)}</span>
        </div>`;
        if (remaining > 0) {
            html += `<div style="display:flex;justify-content:space-between;padding:4px 0;font-weight:600">
                <span>Saldo pendiente</span><span style="color:#ef4444">${formatCurrency(remaining)}</span>
            </div>`;
        }
        html += `</div>`;
        container.innerHTML = html;
    }).catch(() => {
        container.innerHTML = `<div style="padding:10px 14px;background:#fef3c7;border-radius:8px;font-size:13px;color:#92400e">⏳ Pendiente de pago</div>`;
    });
}

let pendingWarrantyDetailId = null;

function claimWarranty(detailId, event) {
    if (event) event.stopPropagation();
    if (!confirm('¿Está seguro de reclamar garantía para este producto?\n\nEsta acción no se puede deshacer.')) return;
    pendingWarrantyDetailId = detailId;
    document.getElementById('sale-detail-modal').style.display = 'none';
    document.getElementById('wm-resolution').value = 'Cambio mismo';
    document.getElementById('wm-replacement-code').value = '';
    document.getElementById('wm-notes').value = '';
    toggleReplacementField();
    document.getElementById('warranty-modal').style.display = 'flex';
    document.getElementById('warranty-modal').classList.add('open');
}

function toggleReplacementField() {
    const val = document.getElementById('wm-resolution').value;
    document.getElementById('wm-replacement-group').style.display = val === 'Cambio otro' ? '' : 'none';
}

function closeWarrantyModal() {
    document.getElementById('warranty-modal').style.display = 'none';
    document.getElementById('warranty-modal').classList.remove('open');
    if (pendingWarrantyDetailId && currentSaleForPayment) {
        document.getElementById('sale-detail-modal').style.display = 'flex';
    }
    pendingWarrantyDetailId = null;
}

async function confirmWarrantyClaim() {
    if (!pendingWarrantyDetailId) return;
    const resolution = document.getElementById('wm-resolution').value;
    const notes = document.getElementById('wm-notes').value.trim() || null;
    try {
        const replacementCode = document.getElementById('wm-replacement-code').value.trim() || null;
        await apiPost(`${API_BASE}/api/sale/details/${pendingWarrantyDetailId}/warranty-claim`, {
            resolution: resolution,
            notes: notes,
            replacementCode: replacementCode
        });
        notify('Reclamo de garantía registrado', 'success');
        closeWarrantyModal();
        if (currentSaleForPayment) viewSaleDetail(currentSaleForPayment.id);
    } catch (e) {
        notify('Error: ' + e.message, 'error');
    }
}



