let currentImages = [], currentIdx = 0;
let currentProductId = null;
let currentProduct = null;
let productsCache = [];

function renderProducts(products) {
    const tbody = document.getElementById("product-table");
    const empty = document.getElementById("empty-state");
    tbody.innerHTML = "";
    const total = products.length;
    const withStock = products.filter(p => p.stock > p.minStock).length;
    const lowStock = products.filter(p => p.stock > 0 && p.stock <= p.minStock).length;
    const noStock = products.filter(p => p.stock === 0).length;
    document.getElementById("stat-total").textContent = total;
    document.getElementById("stat-stock").textContent = withStock;
    document.getElementById("stat-low").textContent = lowStock;
    document.getElementById("stat-zero").textContent = noStock;
    document.getElementById("results-count").textContent = total + " producto" + (total !== 1 ? "s" : "");
    if (total) {
        document.getElementById("bar-stock").style.width = (withStock / total * 100) + "%";
        document.getElementById("bar-low").style.width = (lowStock / total * 100) + "%";
        document.getElementById("bar-zero").style.width = (noStock / total * 100) + "%";
    }
    if (!total) { empty.style.display = ""; return; }
    empty.style.display = "none";
    const frag = document.createDocumentFragment();
    const tvModelFilter = (document.getElementById("tvModel")?.value || "").trim().toLowerCase();
    products.forEach((p, idx) => {
        const led = p.ledDetails;
        const imgUrl = p.images?.length ? (p.images.find(x => x.isMain)?.url || p.images[0].url) : "https://placehold.co/48x48/eef0f8/8b92b0?text=LED";
        let stockClass = 'badge-green';
        if (p.stock === 0) stockClass = 'badge-red';
        else if (p.stock <= p.minStock) stockClass = 'badge-yellow';
        const tr = document.createElement("tr");
        tr.className = 'tr-card';
        tr.style.animationDelay = (idx * 0.04) + 's';
        const modelsHtml = led?.compatibleTVModels?.length
            ? `<div class="td-models">${led.compatibleTVModels.map(m => {
                const match = tvModelFilter && m.toLowerCase().includes(tvModelFilter);
                return `<span class="td-model-chip${match ? ' highlight' : ''}">${esc(m)}</span>`;
            }).join('')}</div>`
            : '';
        tr.innerHTML = `
            <td><img src="${esc(imgUrl)}" class="product-img" loading="lazy"></td>
            <td class="td-code">${esc(p.code)}</td>
            <td class="td-name">
                <div>${esc(p.name)}</div>
                ${modelsHtml}
            </td>
            <td class="td-brand">
                <div style="font-weight:600;color:var(--text)">${esc(p.brand)}</div>
                <div style="font-size:11px;font-family:'Space Mono',monospace;color:var(--muted);margin-top:3px">${esc(p.model)}</div>
            </td>
            <td class="td-led"><span class="chip chip-blue">${led?.ledCount ?? "—"}led/${led?.stripCount ?? "—"}T</span></td>
            <td class="td-volts"><span class="chip chip-gray">${led?.ledVolts ?? "—"}V</span></td>
            <td class="td-inch">${led?.inch ?? "—"}″</td>
            <td class="td-price">${formatCurrency(p.price)}</td>
            <td><span class="badge ${stockClass}">${p.stock} uds</span></td>
        `;
        tr.style.cursor = "pointer";
        tr.addEventListener("click", () => showProduct(p));
        frag.appendChild(tr);
    });
    tbody.appendChild(frag);
}

function showProduct(p) {
    currentProductId = p.id;
    currentProduct = p;
    const led = p.ledDetails;
    _restoreModalHeader();
    document.getElementById("modal-title").textContent = p.name;
    document.getElementById("m-category").textContent = p.categoryName || "Tiras LED";
    document.getElementById("m-code").textContent = p.code;
    document.getElementById("m-brand").textContent = p.brand;
    document.getElementById("m-model").textContent = p.model;
    document.getElementById("m-price").textContent = formatCurrency(p.price);
    const isLow = p.stock <= p.minStock;
    document.getElementById("m-stock").textContent = p.stock;
    document.getElementById("m-stock-box").className = "stock-box " + (isLow ? "low" : "ok");
    document.getElementById("m-stock-badge").className = "sb-badge " + (isLow ? "low" : "ok");
    document.getElementById("m-stock-badge").textContent = isLow ? "⚠ STOCK BAJO" : "✓ DISPONIBLE";
    const orderEl = document.getElementById("m-order-stock");
    if (p.cantOrderStock > 0) { orderEl.textContent = "📦 Pedido: " + p.cantOrderStock + " uds"; orderEl.style.display = ""; }
    else orderEl.style.display = "none";
    document.getElementById("s-inch").innerHTML = (led?.inch ?? "—") + '<span class="sc2-unit">″</span>';
    document.getElementById("s-volts").innerHTML = (led?.ledVolts ?? "—") + '<span class="sc2-unit">V</span>';
    document.getElementById("s-strips").textContent = led?.stripCount ?? "—";
    document.getElementById("s-leds").textContent = led?.ledCount ?? "—";
    document.getElementById("s-length").innerHTML = (led?.lengthMm ? (led.lengthMm / 10) : "—") + '<span class="sc2-unit">cm</span>';
    document.getElementById("s-dist").textContent = led?.distribution ?? "—";
    document.getElementById("s-board").textContent = led?.boardCode ?? "—";
    document.getElementById("s-barcode").textContent = p.barcode ?? "—";
    const nw = document.getElementById("notes-wrap");
    if (led?.notes) { document.getElementById("s-notes").textContent = led.notes; nw.classList.add("visible"); }
    else nw.classList.remove("visible");
    const ds = document.getElementById("desc-sec");
    if (p.description) { document.getElementById("s-desc").textContent = p.description; ds.style.display = ""; }
    else ds.style.display = "none";
    const ml = document.getElementById("model-list");
    const models = led?.compatibleTVModels || [];
    document.getElementById("m-model-count").textContent = models.length;
    ml.innerHTML = "";
    if (models.length) {
        const f = document.createDocumentFragment();
        models.forEach(m => { const c = document.createElement("div"); c.className = "model-card"; c.textContent = m; f.appendChild(c); });
        ml.appendChild(f);
    } else {
        ml.innerHTML = '<span class="models-empty">Sin modelos registrados</span>';
    }
    currentImages = p.images || [];
    currentIdx = Math.max(0, currentImages.findIndex(i => i.isMain));
    buildGallery();
    document.getElementById("modal-footer").innerHTML = `
        <button class="btn-edit" onclick="openEditModal()">✎ Editar producto</button>
        <button class="btn-modal-close" onclick="closeModal()">Cerrar</button>
    `;
    document.querySelector('.gallery-col').style.display = 'flex';
    document.querySelector('.info-col').style.display = 'block';
    document.getElementById('edit-col').style.display = 'none';
    document.getElementById('register-col').style.display = 'none';
    document.getElementById("modal").classList.add("open");
    document.body.style.overflow = "hidden";
}

function buildGallery() {
    const wrap = document.getElementById("thumbnails");
    wrap.innerHTML = "";
    if (!currentImages.length) {
        document.getElementById("main-image").src = "https://placehold.co/280x280/f7f8fc/8b92b0?text=Sin+imagen";
        document.getElementById("img-counter").style.display = "none";
        document.getElementById("main-badge").style.display = "none";
        return;
    }
    setMainImg(currentIdx);
    const f = document.createDocumentFragment();
    currentImages.forEach((img, i) => {
        const d = document.createElement("div");
        d.className = "thumb" + (i === currentIdx ? " active" : "");
        const im = new Image(); im.src = img.url; im.loading = "lazy";
        d.appendChild(im);
        d.addEventListener("click", () => setMainImg(i));
        f.appendChild(d);
    });
    wrap.appendChild(f);
}

function buildGalleryEditMode() {
    const wrap = document.getElementById("thumbnails");
    wrap.innerHTML = "";
    if (!currentImages.length) {
        document.getElementById("main-image").src = "https://placehold.co/280x280/f7f8fc/8b92b0?text=Sin+imagen";
        document.getElementById("img-counter").style.display = "none";
        document.getElementById("main-badge").style.display = "none";
        return;
    }
    setMainImg(currentIdx);
    const f = document.createDocumentFragment();
    currentImages.forEach((img, i) => {
        const d = document.createElement("div");
        d.className = "thumb thumb-editable" + (i === currentIdx ? " active" : "");
        d.style.position = "relative";
        const im = new Image(); im.src = img.url; im.loading = "lazy";
        d.appendChild(im);
        if (img.isMain) {
            const badge = document.createElement("span");
            badge.textContent = "✓";
            badge.title = "Imagen principal";
            badge.style.cssText = "position:absolute;top:2px;left:2px;background:#22c55e;color:#fff;font-size:9px;padding:1px 4px;border-radius:3px;pointer-events:none;";
            d.appendChild(badge);
        }
        const btn = document.createElement("button");
        btn.innerHTML = "✕";
        btn.title = "Eliminar imagen";
        btn.style.cssText = "position:absolute;top:2px;right:2px;background:#ef4444;color:#fff;border:none;border-radius:50%;width:18px;height:18px;font-size:10px;cursor:pointer;display:flex;align-items:center;justify-content:center;line-height:1;padding:0;";
        btn.addEventListener("click", (e) => { e.stopPropagation(); deleteImage(img.publicId, d, i); });
        d.appendChild(btn);
        d.addEventListener("click", () => setMainImg(i));
        f.appendChild(d);
    });
    wrap.appendChild(f);
}

function setMainImg(i) {
    currentIdx = i;
    const img = currentImages[i];
    document.getElementById("main-image").src = img.url;
    const ctr = document.getElementById("img-counter");
    if (currentImages.length > 1) { ctr.textContent = (i + 1) + "/" + currentImages.length; ctr.style.display = ""; }
    else ctr.style.display = "none";
    document.getElementById("main-badge").style.display = img.isMain ? "" : "none";
    document.querySelectorAll(".thumb").forEach((t, j) => t.classList.toggle("active", j === i));
}

function openEditModal() {
    if (!currentProduct) return;
    document.querySelector('.gallery-col').style.display = 'none';
    document.querySelector('.info-col').style.display = 'none';
    document.getElementById('edit-col').style.display = 'flex';
    document.getElementById('register-col').style.display = 'none';
    document.getElementById("modal-footer").innerHTML = `
        <button class="btn-cancel" onclick="closeEditMode()">Cancelar</button>
        <button class="btn-save" onclick="saveProductChanges(this)">💾 Guardar cambios</button>
    `;
    loadProductDataToEdit();
}

function loadProductDataToEdit() {
    const p = currentProduct; if (!p) return;
    const led = p.ledDetails || {};
    document.getElementById('edit-name').value = p.name || '';
    document.getElementById('edit-brand').value = p.brand || '';
    document.getElementById('edit-model').value = p.model || '';
    document.getElementById('edit-barcode').value = p.barcode || '';
    document.getElementById('edit-price').value = p.price || '';
    document.getElementById('edit-stock').value = p.stock || '';
    document.getElementById('edit-minStock').value = p.minStock || '';
    document.getElementById('edit-orderStock').value = p.cantOrderStock ?? '';
    document.getElementById('edit-inch').value = led.inch || '';
    document.getElementById('edit-volts').value = led.ledVolts || '';
    document.getElementById('edit-stripCount').value = led.stripCount || '';
    document.getElementById('edit-ledCount').value = led.ledCount || '';
    document.getElementById('edit-length').value = led.lengthMm || '';
    document.getElementById('edit-distribution').value = led.distribution || '';
    document.getElementById('edit-boardCode').value = led.boardCode || '';
    document.getElementById('edit-models').value = Array.isArray(led.compatibleTVModels) ? led.compatibleTVModels.join(', ') : '';
    document.getElementById('edit-notes').value = led.notes || '';
    document.getElementById('edit-description').value = p.description || '';
    document.getElementById('edit-isActive').checked = p.isActive !== false;
    document.getElementById('edit-code-display').textContent = p.code || '—';
    document.getElementById('edit-ledType').value = String(led.ledType ?? 0);
    document.getElementById('edit-warranty').value = p.defaultWarrantyDays ?? '';
}

function closeEditMode() {
    document.getElementById('edit-col').style.display = 'none';
    document.querySelector('.gallery-col').style.display = 'flex';
    document.querySelector('.info-col').style.display = 'block';
    document.getElementById("modal-footer").innerHTML = `
        <button class="btn-edit" onclick="openEditModal()">✎ Editar producto</button>
        <button class="btn-modal-close" onclick="closeModal()">Cerrar</button>
    `;
}

async function saveProductChanges(saveButton) {
    saveButton.disabled = true;
    const originalText = saveButton.textContent;
    saveButton.textContent = 'Guardando...';
    try {
        const stripCountRaw = parseInt(document.getElementById('edit-stripCount').value);
        const ledVoltsRaw = parseInt(document.getElementById('edit-volts').value);
        const ledTypeRaw = parseInt(document.getElementById('edit-ledType').value);
        const updatedData = {
            name: document.getElementById('edit-name').value.trim(),
            brand: document.getElementById('edit-brand').value.trim(),
            model: document.getElementById('edit-model').value.trim(),
            barcode: document.getElementById('edit-barcode').value.trim() || null,
            price: parseFloat(document.getElementById('edit-price').value) || 0,
            stock: parseInt(document.getElementById('edit-stock').value) || 0,
            minStock: parseInt(document.getElementById('edit-minStock').value) || 0,
            cantOrderStock: parseInt(document.getElementById('edit-orderStock').value) || 0,
            description: document.getElementById('edit-description').value.trim() || null,
            isActive: document.getElementById('edit-isActive').checked,
            defaultWarrantyDays: parseInt(document.getElementById('edit-warranty').value) || null,
            ledDetails: {
                inch: parseInt(document.getElementById('edit-inch').value) || 0,
                stripCount: (stripCountRaw >= 1 && stripCountRaw <= 50) ? stripCountRaw : 1,
                lengthMm: parseInt(document.getElementById('edit-length').value) || null,
                ledCount: parseInt(document.getElementById('edit-ledCount').value) || null,
                ledVolts: (ledVoltsRaw >= 3 && ledVoltsRaw <= 6) ? ledVoltsRaw : null,
                ledType: [0, 1, 2, 3, 4].includes(ledTypeRaw) ? ledTypeRaw : 0,
                boardCode: document.getElementById('edit-boardCode').value.trim() || null,
                distribution: document.getElementById('edit-distribution').value.trim() || null,
                notes: document.getElementById('edit-notes').value.trim() || null,
                compatibleTVModels: document.getElementById('edit-models').value
                    .split(',').map(s => s.trim()).filter(s => s.length > 0)
            }
        };
        if (!updatedData.name || !updatedData.brand) throw new Error('Nombre y marca son requeridos');
        const response = await fetch(`${API_BASE}/api/products/${currentProductId}/info`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'accept': '*/*' },
            body: JSON.stringify(updatedData)
        });
        if (!response.ok) {
            const errText = await response.text();
            throw new Error(`Error al guardar: ${response.status} - ${errText}`);
        }
        if (response.status !== 204) {
            try { currentProduct = await response.json(); } catch { await refreshCurrentProduct(); }
        } else {
            await refreshCurrentProduct();
        }
        showProduct(currentProduct);
        loadAll();
        notify('Producto guardado exitosamente', 'success');
    } catch (error) {
        console.error('Error:', error);
        notify('Error: ' + error.message, 'error');
        saveButton.disabled = false;
        saveButton.textContent = originalText;
    }
}

async function refreshCurrentProduct() {
    try {
        const r = await fetch(`${API_BASE}/api/products/${currentProductId}`);
        if (r.ok) { const json = await r.json(); currentProduct = json.data ?? json; }
    } catch (e) { console.warn('No se pudo refrescar el producto:', e); }
}

function openRegisterModal() {
    currentProduct = null;
    currentProductId = null;
    document.querySelector('.gallery-col').style.display = 'none';
    document.querySelector('.info-col').style.display = 'none';
    document.getElementById('edit-col').style.display = 'none';
    document.getElementById('register-col').style.display = 'flex';
    clearRegisterForm();
    document.querySelector('.modal-header-left').innerHTML = `
        <div class="modal-eyebrow"><span class="dot-live"></span><span>Nuevo producto</span></div>
        <div class="modal-title">Registrar tira LED</div>
        <div class="modal-meta" style="opacity:0.7;">
            <span class="modal-meta-item">Complete todos los campos requeridos (*)</span>
        </div>
    `;
    document.getElementById("modal-footer").innerHTML = `
        <button class="btn-cancel" onclick="closeRegisterMode()">Cancelar</button>
        <button class="btn-save" onclick="saveNewProduct(this)">💾 Guardar producto</button>
    `;
    document.getElementById("modal").classList.add("open");
    document.body.style.overflow = "hidden";
}

function clearRegisterForm() {
    ['register-code', 'register-name', 'register-brand', 'register-model',
        'register-barcode', 'register-description', 'register-distribution',
        'register-boardCode', 'register-notes', 'register-models',
        'register-inch', 'register-stripCount', 'register-ledCount', 'register-length',
        'register-price'
    ].forEach(id => document.getElementById(id).value = '');
    document.getElementById('register-categoryId').value = '1';
    document.getElementById('register-categoryName').value = 'Tiras LED';
    document.getElementById('register-stock').value = '0';
    document.getElementById('register-minStock').value = '2';
    document.getElementById('register-orderStock').value = '0';
    document.getElementById('register-warranty').value = '0';
    document.getElementById('register-volts').value = '6';
    document.getElementById('register-isActive').value = 'true';
    document.getElementById('register-ledType').value = '0';
}

function closeRegisterMode() {
    document.getElementById('register-col').style.display = 'none';
    _restoreModalHeader();
    if (currentProduct) {
        document.querySelector('.gallery-col').style.display = 'flex';
        document.querySelector('.info-col').style.display = 'block';
        document.getElementById("modal-footer").innerHTML = `
            <button class="btn-edit" onclick="openEditModal()">✎ Editar producto</button>
            <button class="btn-modal-close" onclick="closeModal()">Cerrar</button>
        `;
    } else {
        closeModal();
    }
}

async function saveNewProduct(saveButton) {
    saveButton.disabled = true;
    const originalText = saveButton.textContent;
    saveButton.textContent = 'Guardando...';
    try {
        const code = document.getElementById('register-code').value.trim();
        const barcode = document.getElementById('register-barcode').value.trim() || null;
        const name = document.getElementById('register-name').value.trim();
        const categoryId = parseInt(document.getElementById('register-categoryId').value) || 1;
        const categoryName = document.getElementById('register-categoryName').value.trim() || 'Tiras LED';
        const brand = document.getElementById('register-brand').value.trim();
        const model = document.getElementById('register-model').value.trim() || null;
        const price = parseFloat(document.getElementById('register-price').value) || 0;
        const stock = parseInt(document.getElementById('register-stock').value) || 0;
        const minStock = parseInt(document.getElementById('register-minStock').value) || 2;
        const cantOrderStock = parseInt(document.getElementById('register-orderStock').value) || 0;
        const isActive = document.getElementById('register-isActive').value === 'true';
        const description = document.getElementById('register-description').value.trim() || null;
        if (!code) throw new Error('El código del producto es requerido');
        if (!name) throw new Error('El nombre del producto es requerido');
        if (!brand) throw new Error('La marca es requerida');
        if (price <= 0) throw new Error('El precio debe ser mayor a 0');
        const inch = parseInt(document.getElementById('register-inch').value) || 0;
        const stripCountRaw = parseInt(document.getElementById('register-stripCount').value);
        const ledVoltsRaw = parseInt(document.getElementById('register-volts').value);
        const stripCount = (stripCountRaw >= 1 && stripCountRaw <= 50) ? stripCountRaw : 1;
        const ledVolts = (ledVoltsRaw >= 3 && ledVoltsRaw <= 6) ? ledVoltsRaw : null;
        const lengthMm = parseInt(document.getElementById('register-length').value) || null;
        const ledCount = parseInt(document.getElementById('register-ledCount').value) || null;
        const boardCode = document.getElementById('register-boardCode').value.trim() || null;
        const distribution = document.getElementById('register-distribution').value.trim() || null;
        const ledTypeRaw = parseInt(document.getElementById('register-ledType').value);
        const ledType = [0, 1, 2, 3, 4].includes(ledTypeRaw) ? ledTypeRaw : 0;
        const notes = document.getElementById('register-notes').value.trim() || null;
        const compatibleTVModels = document.getElementById('register-models').value
            .split(',').map(s => s.trim()).filter(s => s.length > 0);
        const newProduct = {
            code, barcode, name, categoryId, categoryName,
            brand, model, price, stock, minStock, cantOrderStock, isActive, description,
            images: [],
            defaultWarrantyDays: parseInt(document.getElementById('register-warranty').value) || null,
            ledDetails: {
                inch, stripCount, lengthMm, ledCount, ledVolts,
                boardCode, distribution, ledType, notes,
                compatibleTVModels: compatibleTVModels.length > 0 ? compatibleTVModels : null
            }
        };
        const response = await fetch(`${API_BASE}/api/products`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json', 'accept': '*/*' },
            body: JSON.stringify(newProduct)
        });
        if (!response.ok) {
            let errorMsg = `Error ${response.status}`;
            try {
                const errBody = await response.json();
                if (errBody.errors) {
                    errorMsg = "Errores de validación:\n" + Object.entries(errBody.errors)
                        .map(([k, v]) => `• ${k}: ${v.join(', ')}`).join('\n');
                } else {
                    errorMsg = errBody.message || errBody.title || errorMsg;
                }
            } catch { }
            if (errorMsg.toLowerCase().includes('already exists') || errorMsg.toLowerCase().includes('ya existe')) {
                const codeInput = document.getElementById('register-code');
                codeInput.style.border = '2px solid var(--red, #e53e3e)';
                codeInput.focus();
                codeInput.addEventListener('input', () => codeInput.style.border = '', { once: true });
                throw new Error(`El código "${code}" ya está registrado. Usa un código diferente.`);
            }
            throw new Error(errorMsg);
        }
        const result = await response.json();
        notify('Producto registrado exitosamente', 'success');
        await loadAll();
        const created = result?.data ?? result;
        if (created?.id) { showProduct(created); } else { closeModal(); }
    } catch (error) {
        console.error('Error:', error);
        notify('Error: ' + error.message, 'error');
        saveButton.disabled = false;
        saveButton.textContent = originalText;
    }
}

function closeModal() {
    document.getElementById("modal").classList.remove("open");
    document.body.style.overflow = "";
    currentProduct = null;
    currentProductId = null;
    _restoreModalHeader();
}

function handleModalBg(e) {
    if (e.target === document.getElementById("modal")) closeModal();
}

function _restoreModalHeader() {
    document.querySelector('.modal-header-left').innerHTML = `
        <div class="modal-eyebrow"><span class="dot-live"></span><span id="m-category">Tiras LED</span></div>
        <div class="modal-title" id="modal-title">— </div>
        <div class="modal-meta">
            <span class="modal-meta-item">COD: <span id="m-code" style="color:#ef4444;font-size:20px;font-weight:700;">—</span></span>
            <span class="modal-meta-item">MARCA: <span id="m-brand">—</span></span>
            <span class="modal-meta-item">MODELO: <span id="m-model">—</span></span>
            <button id="btn-open-register" class="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600" onclick="openRegisterModal()">➕ Registrar producto</button>
        </div>
    `;
}

function openLightbox() {
    if (!currentImages.length) return;
    document.getElementById("lightbox-img").src = currentImages[currentIdx].url;
    document.getElementById("lightbox-img").className = "lb-enter";
    updateLightboxMeta();
    const many = currentImages.length > 1;
    document.getElementById("lb-prev").style.display = many ? "flex" : "none";
    document.getElementById("lb-next").style.display = many ? "flex" : "none";
    document.getElementById("lightbox").classList.add("open");
}

function closeLightbox() { document.getElementById("lightbox").classList.remove("open"); }
function handleLightboxBg(e) { if (e.target === document.getElementById("lightbox")) closeLightbox(); }

function lbNav(dir) {
    currentIdx = (currentIdx + dir + currentImages.length) % currentImages.length;
    const el = document.getElementById("lightbox-img");
    el.className = "";
    requestAnimationFrame(() => { el.src = currentImages[currentIdx].url; el.className = "lb-enter"; });
    updateLightboxMeta();
    setMainImg(currentIdx);
}

function updateLightboxMeta() {
    document.getElementById("lb-counter").textContent = (currentIdx + 1) + " / " + currentImages.length;
    document.getElementById("lb-caption").textContent = currentImages[currentIdx].isMain ? "PRINCIPAL" : "IMAGEN " + (currentIdx + 1);
}

function loadAll() {
    return fetch(`${API_BASE}/api/products`)
        .then(r => r.json())
        .then(d => {
            const data = d.data ?? d;
            productsCache = data;
            renderProducts(data);
            return data;
        })
        .catch(() => { productsCache = []; renderProducts([]); });
}

function filterProducts() {
    const search = document.getElementById('search').value.trim().toLowerCase();
    const tvModel = document.getElementById('tvModel').value.trim().toLowerCase();
    const inch = document.getElementById('inch').value.trim();
    const stripCount = document.getElementById('stripCount').value.trim();
    const ledCount = document.getElementById('ledCount').value.trim();
    const volts = document.getElementById('volts').value;
    const ledType = document.getElementById('LedType').value;
    const boardCode = document.getElementById('boardCode').value.trim().toLowerCase();

    const hasAnyFilter = search || tvModel || inch || stripCount || ledCount || volts || ledType || boardCode;
    if (!hasAnyFilter) {
        renderProducts(productsCache);
        return;
    }

    const filtered = productsCache.filter(p => {
        const led = p.ledDetails;

        if (search) {
            const searchRe = new RegExp('(^|\\W)' + search.replace(/[.*+?^${}()|[\]\\]/g, '\\$&') + '(\\W|$)', 'i');
            const nameMatch = p.name.toLowerCase().includes(search);
            const codeMatch = searchRe.test(p.code);
            const catMatch = p.categoryName && p.categoryName.toLowerCase().includes(search);
            if (!nameMatch && !codeMatch && !catMatch) return false;
        }
        if (tvModel) {
            const models = led?.compatibleTVModels || [];
            if (!models.some(m => m.toLowerCase().includes(tvModel))) return false;
        }
        if (inch && led?.inch != parseInt(inch)) return false;
        if (stripCount && led?.stripCount != parseInt(stripCount)) return false;
        if (ledCount && led?.ledCount != parseInt(ledCount)) return false;
        if (volts && led?.ledVolts != parseInt(volts)) return false;
        if (ledType && led?.ledType !== undefined && led.ledType != parseInt(ledType)) return false;
        if (boardCode && !(led?.boardCode && led.boardCode.toLowerCase().includes(boardCode))) return false;

        return true;
    });

    renderProducts(filtered);
}

const filterProductsDebounced = debounce(filterProducts, 300);

function doSearch() {
    const params = new URLSearchParams();
    const v = id => document.getElementById(id).value.trim();
    if (v("search")) params.append("Search", v("search"));
    if (v("tvModel")) params.append("CompatibleTVModel", v("tvModel"));
    if (v("inch")) params.append("Inch", v("inch"));
    if (v("stripCount")) params.append("StripCount", v("stripCount"));
    if (v("ledCount")) params.append("LedCount", v("ledCount"));
    if (v("volts")) params.append("LedVolts", v("volts"));
    if (v("LedType")) params.append("LedType", v("LedType"));
    if (v("boardCode")) params.append("BoardCode", v("boardCode"));

    fetch(`${API_BASE}/api/products/led-strips/search?` + params.toString())
        .then(r => r.json())
        .then(d => {
            const products = d.data ?? d;
            if (!products.length) return renderProducts([]);
            return Promise.all(
                products.map(p =>
                    fetch(`${API_BASE}/api/products/${p.id}`)
                        .then(r => r.json())
                        .then(d => d.data ?? d)
                )
            );
        })
        .then(products => products && renderProducts(products))
        .catch(() => renderProducts([]));
}

function clearSearch() {
    ["search", "tvModel", "inch", "stripCount", "ledCount", "boardCode"].forEach(id => document.getElementById(id).value = "");
    document.getElementById("volts").value = "";
    document.getElementById("LedType").value = "";
    loadAll();
}

async function openImageEditMode() {
    if (!currentProductId) return;
    document.querySelector('.info-col').style.display = 'none';
    document.getElementById('edit-col').style.display = 'none';
    document.getElementById('register-col').style.display = 'none';
    document.querySelector('.gallery-col').style.display = 'flex';
    document.getElementById("modal-footer").innerHTML = `
        <span id="img-status" style="font-size:13px;color:var(--muted);flex:1">Cargando imágenes…</span>
        <label class="btn-edit" style="cursor:pointer">
            📤 Subir nuevas
            <input type="file" id="uploadImagesInput" multiple style="display:none" onchange="uploadImages(this)">
        </label>
        <button class="btn-cancel" onclick="closeImageEditMode()">← Volver</button>
    `;
    try {
        const r = await fetch(`${API_BASE}/api/products/${currentProductId}`);
        if (!r.ok) throw new Error("No se pudo obtener el producto");
        const json = await r.json();
        currentImages = (json.data ?? json).images || [];
        if (currentProduct) currentProduct.images = currentImages;
    } catch (e) {
        document.getElementById("img-status").textContent = "Error al cargar imágenes";
        console.error(e);
        return;
    }
    currentIdx = Math.max(0, currentImages.findIndex(i => i.isMain));
    buildGalleryEditMode();
    const count = currentImages.length;
    document.getElementById("img-status").textContent =
        count === 0 ? "Sin imágenes — sube la primera" : `${count} imagen${count !== 1 ? "es" : ""}`;
}

function closeImageEditMode() {
    document.querySelector('.gallery-col').style.display = 'flex';
    document.querySelector('.info-col').style.display = 'block';
    document.getElementById('edit-col').style.display = 'none';
    buildGallery();
    document.getElementById("modal-footer").innerHTML = `
        <button class="btn-edit" onclick="openEditModal()">✎ Editar producto</button>
        <button class="btn-modal-close" onclick="closeModal()">Cerrar</button>
    `;
}

async function uploadImages(inputEl) {
    const files = inputEl.files;
    if (!files.length) return;
    const statusEl = document.getElementById("img-status");
    if (statusEl) statusEl.textContent = "Subiendo…";
    const formData = new FormData();
    for (let i = 0; i < files.length; i++) formData.append("files", files[i]);
    try {
        const r = await fetch(`${API_BASE}/api/products/${currentProductId}/images`, { method: "POST", body: formData });
        if (!r.ok) throw new Error(await r.text());
        const images = await r.json();
        currentImages = images;
        if (currentProduct) currentProduct.images = images;
        currentIdx = Math.max(0, currentImages.findIndex(i => i.isMain));
        buildGalleryEditMode();
        const count = currentImages.length;
        if (statusEl) statusEl.textContent = `${count} imagen${count !== 1 ? "es" : ""}`;
        loadAll();
    } catch (err) {
        notify("Error al subir imágenes: " + err.message, 'error');
        if (statusEl) statusEl.textContent = "Error al subir";
    }
    inputEl.value = "";
}

async function deleteImage(publicId, thumbEl, removedIdx) {
    if (!confirm("¿Eliminar esta imagen?")) return;
    const statusEl = document.getElementById("img-status");
    try {
        const r = await fetch(
            `${API_BASE}/api/products/${currentProductId}/images?publicId=${encodeURIComponent(publicId)}`,
            { method: "DELETE" }
        );
        if (!r.ok) { const text = await r.text(); throw new Error(text || "No se pudo eliminar la imagen"); }
        const json = await r.json();
        if (!json.success) throw new Error(json.message || "Eliminación fallida");
        currentImages.splice(removedIdx, 1);
        if (currentProduct) currentProduct.images = [...currentImages];
        if (currentImages.length === 0) {
            currentIdx = 0;
        } else {
            currentIdx = Math.min(currentIdx, currentImages.length - 1);
            if (!currentImages.some(i => i.isMain) && currentImages.length > 0) currentIdx = 0;
        }
        buildGalleryEditMode();
        const count = currentImages.length;
        if (statusEl) statusEl.textContent =
            count === 0 ? "Sin imágenes — sube la primera" : `${count} imagen${count !== 1 ? "es" : ""}`;
        loadAll();
    } catch (err) {
        notify("Error al eliminar imagen: " + err.message, 'error');
    }
}

function playClickSound() {
    try {
        const ctx = new (window.AudioContext || window.webkitAudioContext)();
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.connect(gain);
        gain.connect(ctx.destination);
        osc.type = "sine";
        osc.frequency.setValueAtTime(800, ctx.currentTime);
        osc.frequency.exponentialRampToValueAtTime(600, ctx.currentTime + 0.08);
        gain.gain.setValueAtTime(0.3, ctx.currentTime);
        gain.gain.exponentialRampToValueAtTime(0.01, ctx.currentTime + 0.1);
        osc.start(ctx.currentTime);
        osc.stop(ctx.currentTime + 0.1);
    } catch {}
}

async function refreshProductsByIds(ids) {
    if (!ids.length) return;
    try {
        const updates = await Promise.all(
            ids.map(id =>
                fetch(`${API_BASE}/api/products/${id}`)
                    .then(r => r.json())
                    .then(d => d.data ?? d)
                    .catch(() => null)
            )
        );
        const valid = updates.filter(Boolean);
        if (!valid.length) return;
        valid.forEach(p => {
            const existing = productsCache.find(x => x.id === p.id);
            if (existing) Object.assign(existing, p);
            else productsCache.push(p);
        });
        renderProducts(productsCache);
    } catch {}
}

function openRemoveStock() {
    if (!currentProduct || !currentProductId) return;
    playClickSound();
    addToCart(currentProductId);
}


