var Auth = Auth || {};
(function ($, undefined) {
    var defaults = {
        success: function (action) {
            window.location.reload(true);
        },
        error: function (err, action) {
            window.location.reload(true);
        },
        loginSelector: '.login',
        logoutSelector: '.logout',
        loginUrl: '/auth/login',
        logoutUrl: '/auth/logout'
    };
    for (var k in defaults) {
        if (typeof Auth[k] !== typeof defaults[k]) {
            Auth[k] = defaults[k];
        }
    }

    $(function () {
        $(Auth.loginSelector).click(function () { navigator.id.request(); });
        $(Auth.logoutSelector).click(function () { navigator.id.logout(); });
    });

    var token = { "__RequestVerificationToken": $("[name='__RequestVerificationToken']").val() };

    navigator.id.watch({
        loggedInUser: Auth.loggedInUser,
        onlogin: function (assertion) {
            $.ajax({
                type: 'POST',
                url: Auth.loginUrl,
                data: $.extend({ assertion: assertion }, token),
                success: function (res, status, xhr) { Auth.success('login'); },
                error: function (xhr, status, err) {
                    navigator.id.logout();
                    Auth.error(err, 'login');
                }
            });
        },
        onlogout: function () {
            $.ajax({
                type: 'POST',
                url: Auth.logoutUrl,
                data: $.extend({}, token),
                success: function (res, status, xhr) { Auth.success('logout'); },
                error: function (xhr, status, err) {
                    Auth.error(err, 'logout');
                }
            });
        }
    });
})(jQuery);
