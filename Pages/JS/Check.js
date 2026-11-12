document.addEventListener("DOMContentLoaded", () => {
    
    // 1. Lógica de cierre de sesión (igual que en Home)
    document.getElementById("btnCerrarSesion").addEventListener("click", async () => {
        try {
            await fetch("/api/logout", { method: "POST", credentials: "include" });
        } catch (err) {
            console.error("Error al salir", err);
        } finally {
            window.location.href = "login.html";
        }
    });

    // 2. Lógica para Validar el Código
    document.getElementById("btnValidar").addEventListener("click", async () => {
        const inputCodigo = document.getElementById("inputCodigo").value.trim();
        const divResultado = document.getElementById("resultadoValidacion");
        const divInfo = document.getElementById("infoVehiculo");

        // Limpiamos los mensajes anteriores
        divResultado.textContent = "";
        divResultado.style.color = "black";
        divInfo.innerHTML = "";

        if (!inputCodigo) {
            divResultado.textContent = "Por favor, ingrese un código.";
            divResultado.style.color = "red";
            return;
        }

        divResultado.textContent = "Validando código...";

        try {
            const res = await fetch("/validar-acceso", {
                method: "POST",
                credentials: "include", // Envía la cookie JWT del guardia
                headers: {
                    "Content-Type": "application/json"
                },
                // VITAL: stringify convierte el texto suelto en un JSON válido ("texto")
                body: JSON.stringify(inputCodigo)
            });

            // Si el token es inválido o el usuario NO tiene rol "Seguridad"
            if (res.status === 401 || res.status === 403) {
                divResultado.textContent = "Acceso denegado. No tienes permisos de guardia.";
                divResultado.style.color = "red";
                return;
            }

            // Si el código fue alterado, expiró, o tiene formato incorrecto (Cae en el catch del C#)
            if (!res.ok) {
                const errText = await res.text();
                divResultado.textContent = `Código Inválido o Rechazado: ${errText}`;
                divResultado.style.color = "red";
                return;
            }

            // Si el servidor regresó un 200 OK
            const mensaje = await res.text();
            
            divResultado.style.display = "block";
            divResultado.style.backgroundColor = "#d4edda";
            divResultado.style.color = "#155724";

            divResultado.innerHTML = `
                <h2>✅ Éxito</h2>
                <p>${mensaje}</p>
            `;

            // Opcional: Limpiar la caja de texto para el siguiente vehículo
            document.getElementById("inputCodigo").value = "";

        } catch (err) {
            divResultado.textContent = "Error de red: " + err.message;
            divResultado.style.color = "red";
            console.error(err);
        }
    });
});