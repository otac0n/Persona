To complete installation of the Persona authentication provider, add the following to all pages that need to log in / out (either through a layout page or individually):

    @Html.AntiForgeryToken()
    <script>
        Auth = Auth || {};
        Auth.loggedInUser = @Html.Raw(Json.Encode(User.Identity.IsAuthenticated ? User.Identity.Name : null));
    </script>
    <script src="https://login.persona.org/include.js"></script>
    <script src="/scripts/persona.js"></script>

Next, update the settings in App_Settings/PersonaConfig.cs to match your installation.

Finally, you can also set Auth.error to handle any errors that may occur during the client-side authentication procedures:

    Auth.error = function (err, action) {
        alert(action + ': ' + err);
        window.location.reload();
    }
