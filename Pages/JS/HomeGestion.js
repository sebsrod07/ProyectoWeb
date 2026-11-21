document.addEventListener("DOMContentLoaded", () => {
    let listaBajas = [];
    let listaAltas = [];

    const gridBajas = document.getElementById("gridVehiculosBaja");
    const gridAltas = document.getElementById("gridVehiculosAlta");
    const contadorBajas = document.getElementById("contadorBajas");
    const contadorAltas = document.getElementById("contadorAltas");
    const estadoCargaBajas = document.getElementById("estadoCargaBajas");
    const estadoCargaAltas = document.getElementById("estadoCargaAltas");
    const inputFiltro = document.getElementById("inputFiltro");
    const mensajeGlobal = document.getElementById("mensajeGlobal");

    // 1. Cierre de sesión
    document.getElementById("btnCerrarSesion").addEventListener("click", async () => {
        try {
            await fetch("/api/logout", { method: "POST", credentials: "include" });
        } catch (err) {
            console.error("Error al cerrar sesión:", err);
        } finally {
            window.location.href = "login.html";
        }
    });

    // 2. Botones de refrescar
    document.getElementById("btnRecargar").addEventListener("click", cargarTodosLosVehiculos);
    document.getElementById("btnActualizarSidebar").addEventListener("click", cargarTodosLosVehiculos);

    // 3. Filtro en tiempo real
    inputFiltro.addEventListener("input", () => {
        const texto = inputFiltro.value.trim().toLowerCase();
        renderizarGrids(texto);
    });

    // Carga inicial
    cargarTodosLosVehiculos();

    async function cargarTodosLosVehiculos() {
        mostrarMensaje("", false);
        if (estadoCargaBajas) estadoCargaBajas.style.display = "block";
        if (estadoCargaAltas) estadoCargaAltas.style.display = "block";

        try {
            const [resBajas, resAltas] = await Promise.all([
                fetch("/getVehiculos?estatus=BAJA", { method: "GET", credentials: "include" }),
                fetch("/getVehiculos?estatus=ALTA", { method: "GET", credentials: "include" })
            ]);

            if (resBajas.status === 401 || resAltas.status === 401 || resBajas.status === 403 || resAltas.status === 403) {
                alert("Sesión no válida o no tienes permisos de Gestión.");
                window.location.href = "login.html";
                return;
            }

            if (!resBajas.ok) throw new Error("Error al obtener vehículos dados de baja.");
            if (!resAltas.ok) throw new Error("Error al obtener vehículos activos.");

            listaBajas = await resBajas.json();
            listaAltas = await resAltas.json();

            renderizarGrids(inputFiltro.value.trim().toLowerCase());

        } catch (err) {
            mostrarMensaje("Error al conectar con el servidor: " + err.message, true);
        }
    }

    function renderizarGrids(filtro = "") {
        renderizarGridBajas(filtro);
        renderizarGridAltas(filtro);
    }

    function renderizarGridBajas(filtro = "") {
        gridBajas.innerHTML = "";

        const filtrados = listaBajas.filter(v => coincideFiltro(v, filtro));
        contadorBajas.textContent = filtrados.length;

        if (filtrados.length === 0) {
            gridBajas.innerHTML = "<p style='color: #ffffff; text-align: center; width: 100%; font-size: 1.1rem;'>No hay vehículos dados de baja o pendientes.</p>";
            return;
        }

        filtrados.forEach(v => {
            const card = document.createElement("div");
            card.className = "vehiculo-card";
            card.dataset.placa = v.placas.trim();

            card.innerHTML = `
                <div class="placa">
                    ${v.placas.trim()}
                    <span class="badge-estatus badge-baja">BAJA</span>
                </div>
                <p><strong>Descripción:</strong> ${v.descripcion}</p>
                <p><strong>Tipo:</strong> ${v.tipo}</p>
                <p><strong>No. Serie:</strong> ${v.numeroSerie}</p>
                <hr>
                <p><small><strong>Propietario:</strong> ${v.nombreCompleto}</small></p>
                <p><small><strong>CURP:</strong> ${v.curp}</small></p>
                <button type="button" class="btn-cyber btn-dar-alta" style="width: 100%; margin-top: 10px;">
                    Dar de Alta
                    <span class="btn-cyber-border"></span>
                </button>
                <p class="status-accion" style="margin-top: 8px; font-weight: bold; font-size: 0.9em;"></p>
            `;

            const btnAlta = card.querySelector(".btn-dar-alta");
            const statusAccion = card.querySelector(".status-accion");

            btnAlta.addEventListener("click", async () => {
                btnAlta.disabled = true;
                statusAccion.style.color = "#0284c7";
                statusAccion.textContent = "Procesando alta...";

                try {
                    const res = await fetch(`/altaVehiculo?placa=${encodeURIComponent(v.placas.trim())}`, {
                        method: "POST",
                        credentials: "include"
                    });

                    if (!res.ok) {
                        const errText = await res.text();
                        throw new Error(errText || "No se pudo dar de alta el vehículo.");
                    }

                    mostrarMensaje(`¡Vehículo con placa ${v.placas.trim()} dado de alta correctamente!`, false);

                    // Mover de listaBajas a listaAltas
                    v.estatus = "ALTA";
                    listaBajas = listaBajas.filter(item => item.placas.trim() !== v.placas.trim());
                    listaAltas.unshift(v);

                    renderizarGrids(inputFiltro.value.trim().toLowerCase());

                } catch (error) {
                    statusAccion.style.color = "#dc2626";
                    statusAccion.textContent = "Error: " + error.message;
                    btnAlta.disabled = false;
                }
            });

            gridBajas.appendChild(card);
        });
    }

    function renderizarGridAltas(filtro = "") {
        gridAltas.innerHTML = "";

        const filtrados = listaAltas.filter(v => coincideFiltro(v, filtro));
        contadorAltas.textContent = filtrados.length;

        if (filtrados.length === 0) {
            gridAltas.innerHTML = "<p style='color: #ffffff; text-align: center; width: 100%; font-size: 1.1rem;'>No hay vehículos activos registrados.</p>";
            return;
        }

        filtrados.forEach(v => {
            const card = document.createElement("div");
            card.className = "vehiculo-card";
            card.dataset.placa = v.placas.trim();

            card.innerHTML = `
                <div class="placa">
                    ${v.placas.trim()}
                    <span class="badge-estatus badge-alta">ALTA</span>
                </div>
                <p><strong>Descripción:</strong> ${v.descripcion}</p>
                <p><strong>Tipo:</strong> ${v.tipo}</p>
                <p><strong>No. Serie:</strong> ${v.numeroSerie}</p>
                <hr>
                <p><small><strong>Propietario:</strong> ${v.nombreCompleto}</small></p>
                <p><small><strong>CURP:</strong> ${v.curp}</small></p>
                <button type="button" class="btn-cyber btn-accion-baja btn-dar-baja" style="width: 100%; margin-top: 10px;">
                    Dar de Baja
                    <span class="btn-cyber-border"></span>
                </button>
                <p class="status-accion" style="margin-top: 8px; font-weight: bold; font-size: 0.9em;"></p>
            `;

            const btnBaja = card.querySelector(".btn-dar-baja");
            const statusAccion = card.querySelector(".status-accion");

            btnBaja.addEventListener("click", async () => {
                const confirmar = confirm(`¿Está seguro de que desea dar de baja al vehículo con placa ${v.placas.trim()}?`);
                if (!confirmar) return;

                btnBaja.disabled = true;
                statusAccion.style.color = "#0284c7";
                statusAccion.textContent = "Procesando baja...";

                try {
                    const res = await fetch(`/bajaVehiculo?placa=${encodeURIComponent(v.placas.trim())}`, {
                        method: "POST",
                        credentials: "include"
                    });

                    if (!res.ok) {
                        const errText = await res.text();
                        throw new Error(errText || "No se pudo dar de baja el vehículo.");
                    }

                    mostrarMensaje(`Vehículo con placa ${v.placas.trim()} dado de baja correctamente.`, false);

                    // Mover de listaAltas a listaBajas
                    v.estatus = "BAJA";
                    listaAltas = listaAltas.filter(item => item.placas.trim() !== v.placas.trim());
                    listaBajas.unshift(v);

                    renderizarGrids(inputFiltro.value.trim().toLowerCase());

                } catch (error) {
                    statusAccion.style.color = "#dc2626";
                    statusAccion.textContent = "Error: " + error.message;
                    btnBaja.disabled = false;
                }
            });

            gridAltas.appendChild(card);
        });
    }

    function coincideFiltro(v, texto) {
        if (!texto) return true;
        const placa = (v.placas || "").toLowerCase();
        const descripcion = (v.descripcion || "").toLowerCase();
        const tipo = (v.tipo || "").toLowerCase();
        const nombre = (v.nombreCompleto || "").toLowerCase();
        const curp = (v.curp || "").toLowerCase();
        const numSerie = (v.numeroSerie || "").toLowerCase();

        return placa.includes(texto) ||
               descripcion.includes(texto) ||
               tipo.includes(texto) ||
               nombre.includes(texto) ||
               curp.includes(texto) ||
               numSerie.includes(texto);
    }

    function mostrarMensaje(texto, esError = false) {
        if (!texto) {
            mensajeGlobal.style.display = "none";
            mensajeGlobal.textContent = "";
            return;
        }

        mensajeGlobal.textContent = texto;
        mensajeGlobal.style.display = "block";

        if (esError) {
            mensajeGlobal.style.backgroundColor = "rgba(239, 68, 68, 0.2)";
            mensajeGlobal.style.color = "#fecaca";
            mensajeGlobal.style.border = "1px solid #ef4444";
        } else {
            mensajeGlobal.style.backgroundColor = "rgba(34, 197, 94, 0.2)";
            mensajeGlobal.style.color = "#bbf7d0";
            mensajeGlobal.style.border = "1px solid #22c55e";
        }

        // Auto ocultar después de 4 segundos
        setTimeout(() => {
            mensajeGlobal.style.display = "none";
        }, 4000);
    }
});
