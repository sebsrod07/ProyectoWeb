document.getElementById("Registro").addEventListener("submit", async function (e) {
    e.preventDefault(); 

    const mensaje = document.getElementById("mensaje");
    mensaje.textContent = "Registrando...";

    const formData = new FormData(e.target);
    const data = Object.fromEntries(formData.entries());

    // 1. Separamos el texto usando el espacio como divisor
    const nombresArray = data.nombres.trim().split(" ");
    
    // 2. El primer elemento es el Primer Nombre
    data.primerNombre = nombresArray[0];
    
    // 3. Si hay más palabras, las unimos de nuevo para el Segundo Nombre, si no, lo mandamos vacío
    data.segundoNombre = nombresArray.length > 1 ? nombresArray.slice(1).join(" ") : null;
    
    // 4. Borramos la propiedad "nombres" original
    delete data.nombres;

    try {
        const res = await fetch("/api/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data) 
        });

        if (!res.ok) {
            const errText = await res.text();
            mensaje.textContent = "Error al registrar: " + errText;
            return;
        }

        mensaje.textContent = "¡Registro exitoso! Redirigiendo al login...";
        
        setTimeout(() => {
            window.location.href = "login.html";
        }, 1500);

    } catch (err) {
        mensaje.textContent = "Error de red: " + err.message;
    }
});
function redirectLogin()
{
    window.location.replace("login.html");
}