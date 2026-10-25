document.addEventListener("DOMContentLoaded", cargarVehiculos);

    // 2. Lógica de cierre de sesión (Actualizada para cookies)
    document.getElementById("btnCerrarSesion").addEventListener("click", async () => {
      try {
        // Le pedimos al servidor que borre la cookie HttpOnly
        await fetch("/api/logout", { 
          method: "POST",
          credentials: "include" 
        });
      } catch (err) {
        console.error("Error cerrando sesión en el servidor:", err);
      } finally {
        // Siempre redirigimos al login, incluso si falla la red
        window.location.href = "login.html";
      }
    });

    // 3. Función principal para obtener y dibujar los datos
    async function cargarVehiculos() {
      const contenedor = document.getElementById("contenedorVehiculos");
      const estado = document.getElementById("estadoCarga");

      try {
        const res = await fetch("/prueba1", {
          method: "GET",
          credentials: "include", // OBLIGATORIO: Le dice al navegador que envíe tu cookie
          headers: {
            "Content-Type": "application/json"
          }
        });

        // Si el token no existe, expiró o es inválido, .NET devolverá 401
        if (res.status === 401) {
          alert("Tu sesión ha expirado o no estás autorizado. Por favor, inicia sesión de nuevo.");
          window.location.href = "login.html";
          return;
        }

        if (!res.ok) {
          throw new Error("Error en el servidor al obtener los vehículos");
        }

        const vehiculos = await res.json();
        
        estado.style.display = "none"; // Ocultar mensaje de carga

        if (vehiculos.length === 0) {
          contenedor.innerHTML = "<p>No tienes vehículos registrados actualmente.</p>";
          return;
        }

        // Dibujar cada vehículo en el HTML
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
          `;
          
          contenedor.appendChild(card);
        });

      } catch (err) {
        estado.textContent = "Error de conexión: " + err.message;
      }
    }