const TOKEN_KEY = "jwtToken";

    document.getElementById("loginForm").addEventListener("submit", async function (e) {
      e.preventDefault();
      const mensaje = document.getElementById("mensaje");
      mensaje.textContent = "Enviando…";

      const email = document.getElementById("email").value.trim();
      const password = document.getElementById("password").value;
      try {
        const res = await fetch("/api/login", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ email: email, password: password }),
        });

        if (!res.ok) {
          const errText = await res.text();
          mensaje.textContent = errText || "No se pudo iniciar sesión.";
          return;
        }

        let token = (await res.text()).trim();
        if (token.startsWith('"') && token.endsWith('"')) {
          token = JSON.parse(token);
        }
        localStorage.setItem(TOKEN_KEY, token);
        window.location.href="/Pages/HTML/Home.html"
      } catch (err) {
        mensaje.textContent = "Error de red: " + err.message;
      }
    });
    function redirectCreateAccount()
    {
      window.location.replace("SignIn.html");
    }