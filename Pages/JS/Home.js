document.addEventListener("DOMContentLoaded", cargarVehiculos);

// 2. Lógica de cierre de sesión (Actualizada para cookies)
document.getElementById("btnCerrarSesion").addEventListener("click", async () => {
    try {
        await fetch("/api/logout", { 
            method: "POST",
            credentials: "include" 
        });
    } catch (err) {
        console.error("Error cerrando sesión en el servidor:", err);
    } finally {
        window.location.href = "login.html";
    }
});

// 3. Función principal para obtener y dibujar los datos
async function cargarVehiculos() {
    const contenedor = document.getElementById("contenedorVehiculos");
    const estado = document.getElementById("estadoCarga");

    try {
        const res = await fetch("/getVehiculos", {
            method: "GET",
            credentials: "include", // OBLIGATORIO: Le dice al navegador que envíe tu cookie
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (res.status === 401) {
            alert("Tu sesión ha expirado o no estás autorizado. Por favor, inicia sesión de nuevo.");
            window.location.href = "login.html";
            return;
        }

        if (!res.ok) {
            throw new Error("Error en el servidor al obtener los vehículos");
        }

        const vehiculos = await res.json();
        
        estado.style.display = "none"; 

        if (vehiculos.length === 0) {
            contenedor.innerHTML = "<p style='color: #ffffff; font-size: 1.15rem; text-align: center; width: 100%;'>No tienes vehículos registrados actualmente.</p>";
            return;
        }

        vehiculos.forEach(vehiculo => {
            const card = document.createElement("div");
            card.className = "vehiculo-card";
            
            card.innerHTML = `
    <div class="placa">${vehiculo.placas}</div>
    <p><strong>Descripción:</strong> ${vehiculo.descripcion}</p>
    <p><strong>Tipo:</strong> ${vehiculo.tipo}</p>
    <p><strong>No. Serie:</strong> ${vehiculo.numeroSerie}</p>
    <hr>
    <p><small>Propietario: ${vehiculo.nombreCompleto}</small></p>
    <button type="button" class="btn-cyber">
        Solicitar Salida
        <span class="btn-cyber-border"></span>
    </button>
    <p class="status-salida" role="status" style="font-weight: bold;"></p>
    <div class="codigo-salida"></div>
`;

            const btnSolicitar = card.querySelector(".btn-cyber");
            const divSalida = card.querySelector(".codigo-salida");
            const statusSalida = card.querySelector(".status-salida");

            btnSolicitar.addEventListener("click", async () => {
                statusSalida.textContent = "Enviando Solicitud...";
                statusSalida.style.color = "#00d4ff";
                divSalida.innerHTML = ""; // Limpiamos si había un código anterior
                
                try {
                    const peticionRes = await fetch(`/generar-acceso?placa=${vehiculo.placas}`, {
                        method: "GET",
                        credentials: "include",
                        headers: {
                            "Content-Type": "application/json"
                        }
                    });

                    if (!peticionRes.ok) {
                        const errText = await peticionRes.text();
                        statusSalida.textContent = errText || "No se pudo solicitar acceso.";
                        statusSalida.style.color = "#f87171";
                        return;
                    }

                    const data = await peticionRes.json();
                    
                    statusSalida.textContent = "¡Código generado correctamente!";
                    statusSalida.style.color = "#4ade80";
                    
                    divSalida.innerHTML = `<p style="word-wrap: break-word; font-size: 0.9em; background: rgba(0, 212, 255, 0.15); color: #ffffff; padding: 10px; border-radius: 6px; border: 1px solid rgba(0, 212, 255, 0.35);">${data.codigoSeguro}</p>`;
                    
                } catch(err) {
                    statusSalida.textContent = "Error de red: " + err.message;
                    statusSalida.style.color = "#f87171";
                }
            });
            
            contenedor.appendChild(card);
        });
        
    } catch (err) {
        estado.textContent = "Error de conexión: " + err.message;
        estado.style.color = "#f87171";
    }
}
function redirectRegistrarV()
{
    window.location.replace("regVehiculo.html");
}