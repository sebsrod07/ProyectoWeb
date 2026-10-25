document.getElementById("formVehiculo").addEventListener("submit", async function (e) {
      e.preventDefault();
      const mensaje = document.getElementById("mensaje");
      mensaje.textContent = "Procesando...";

      // Extrae automáticamente todos los inputs usando el atributo 'name'
      const formData = new FormData(e.target);
      const data = Object.fromEntries(formData.entries());

      try {
        const res = await fetch("/solicitarRegistro", {
          method: "POST",
          credentials: "include", // Obligatorio para enviar la cookie JWT
          headers: {
            "Content-Type": "application/json"
          },
          body: JSON.stringify(data)
        });
        if (res.status === 401) {
          mensaje.textContent = "Error: Sesión expirada. Vuelve a iniciar sesión.";
          return;
        }

        if (!res.ok) {
          const errText = await res.text();
          mensaje.textContent = "Error al guardar: " + errText;
          return;
        }

        mensaje.textContent = "¡Vehículo registrado correctamente!";
        e.target.reset(); 

      } catch (err) {
        mensaje.textContent = "Error de red: " + err.message;
      }
    });