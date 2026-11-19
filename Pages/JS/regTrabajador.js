document.getElementById("Registro").addEventListener("submit", async function (e) {
    e.preventDefault(); 

    const mensaje = document.getElementById("mensaje");
    mensaje.textContent = "Registrando empleado...";

    const formData = new FormData(e.target);
    const data = Object.fromEntries(formData.entries());

    // 1. Extraemos y eliminamos el rol del objeto (en C# viaja por query string, no en el body)
    const rol = data.rol;
    delete data.rol;

    // 2. Separamos el texto usando el espacio como divisor
    const nombresArray = data.nombres.trim().split(" ");
    data.primerNombre = nombresArray[0];
    data.segundoNombre = nombresArray.length > 1 ? nombresArray.slice(1).join(" ") : null;
    delete data.nombres;

    // 3. Si el RFC viene vacío, lo mandamos como null
    if (data.RFC.trim() === "") {
        data.RFC = null;
    }

    // 4. Convertimos a mayúsculas todos los textos excepto email y password
    Object.keys(data).forEach(key => {
        if (typeof data[key] === "string" && key !== "email" && key !== "password") {
            data[key] = data[key].trim().toUpperCase();
        }
    });

    try {
        const res = await fetch(`/api/registerTrabajador?rol=${encodeURIComponent(rol)}`, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data) 
        });

        if (res.status === 401 || res.status === 403) {
            mensaje.textContent = "No tienes permisos de Administrador o tu sesión expiró.";
            return;
        }

        if (!res.ok) {
            const errText = await res.text();
            mensaje.textContent = "Error al registrar: " + errText;
            return;
        }

        mensaje.textContent = "¡Empleado registrado con éxito!";
        e.target.reset();

    } catch (err) {
        mensaje.textContent = "Error de red: " + err.message;
    }
});

function redirectAdmin() {
    window.location.replace("HomeAdmin.html");
}