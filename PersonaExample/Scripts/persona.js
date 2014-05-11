Auth = Auth || {};
(function ($, undefined) {
    $(function () {
        $('.login').click(function () { navigator.id.request(); });
        $('.logout').click(function () { navigator.id.logout(); });
    });

    var token = { "__RequestVerificationToken": $("[name='__RequestVerificationToken']").val() };

    navigator.id.watch({
        loggedInUser: Auth.loggedInUser,
        onlogin: function (assertion) {
            $.ajax({
                type: 'POST',
                url: '/auth/login',
                data: $.extend({}, token, { assertion: assertion }),
                success: function (res, status, xhr) { window.location.reload(); },
                error: function (xhr, status, err) {
                    navigator.id.logout();
                    if (typeof Auth.error === "function") {
                        Auth.error(err, 'login');
                    }
                }
            });
        },
        onlogout: function () {
            $.ajax({
                type: 'POST',
                url: '/auth/logout',
                data: $.extend({}, token),
                success: function (res, status, xhr) { window.location.reload(); },
                error: function (xhr, status, err) {
                    if (typeof Auth.error === "function") {
                        Auth.error(err, 'logout');
                    }
                }
            });
        }
    });
})(jQuery);
