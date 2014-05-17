To complete installation of the Persona authentication provider, add the following to all pages that need to log in / out (either through a layout page or individually):

    <script>
        var Auth = Auth || {};
        Auth.loggedInUser = @Html.Raw(Json.Encode(User.Identity.IsAuthenticated ? User.Identity.Name : null));
    </script>
    <script src="https://login.persona.org/include.js"></script>
    <script src="/scripts/persona.js"></script>
    @Html.AntiForgeryToken() @* Optional.  Reduces round-trips and speeds up login, but is not cacheable. *@
    <script>
        // Example:
        $(function () {
            $(".login").click(function () { navigator.id.request(); });
            $(".logout").click(function () { navigator.id.logout(); });
        });
    </script>

Next, update the settings in App_Settings/PersonaConfig.cs to match your installation.

Finally, you may set Auth.error or Auth.success to handle any special cases that may occur during the client-side authentication process:

    Auth.error = function (err, action) {
        alert(action + ': ' + err);
        window.location.reload();
    }

    Auth.success = function (action) {
        window.location = @Html.Raw(Json.Encode(returnUrl));
    }
