(() => {
  const bootErrorMarkup = [
    '<main class="boot-error-shell">',
    '  <section class="boot-error-card" role="alert" aria-live="assertive">',
    '    <h1>Erro ao iniciar o ChessSharp</h1>',
    '    <p>Nao foi possivel carregar a aplicacao neste momento. Tente recarregar a pagina em alguns instantes.</p>',
    '  </section>',
    '</main>'
  ].join("");

  function showBootError() {
    const host = document.getElementById("app");
    const errorUi = document.getElementById("blazor-error-ui");

    if (host) {
      host.innerHTML = bootErrorMarkup;
    }

    if (errorUi) {
      errorUi.style.display = "block";
    }
  }

  window.addEventListener("unhandledrejection", (event) => {
    console.error("Unhandled promise rejection during ChessSharp Web startup.", event?.reason);
    showBootError();
  });

  Blazor.start()
    .catch((error) => {
      console.error("ChessSharp Web failed to start.", error);
      showBootError();
    });
})();
