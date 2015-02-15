var Auth = Auth || {};
(function ($, undefined) {
    var defaults = {
        success: function (action) {
            window.location.reload(true);
        },
        error: function (err, action) {
            window.location.reload(true);
        },
        loginUrl: '/auth/login',
        logoutUrl: '/auth/logout',
        tokenUrl: '/auth/token'
    };
    for (var k in defaults) {
        if (typeof Auth[k] !== typeof defaults[k]) {
            Auth[k] = defaults[k];
        }
    }

    var token = { "__RequestVerificationToken": $("[name='__RequestVerificationToken']").val() };

    var freshenToken = function (settings) {
        $.ajax({
            type: 'GET',
            url: Auth.tokenUrl,
            dataType: 'html',
            success: function (res, status, xhr) {
                token["__RequestVerificationToken"] = $(res).val();
                settings.success();
            },
            error: settings.error
        });
    }

    var tryRequest = function (settings, allowRetry) {
        if (allowRetry && !token["__RequestVerificationToken"]) {
            freshenToken({
                success: function () { tryRequest(settings, true); },
                error: function (xhr, status, err) { settings.error(xhr, status, 'Could not obtain an Anti-CSRF token: ' + err); }
            });

            return;
        }

        $.ajax($.extend({}, settings, {
            data: $.extend({}, settings.data, token),
            error: function (xhr, status, err) {
                if (allowRetry) {
                    freshenToken({
                        success: function () { tryRequest(settings, false); },
                        error: function () { settings.error(xhr, status, err); }
                    });
                } else {
                    settings.error(xhr, status, err);
                }
            }
        }));
    }

    navigator.id.watch({
        loggedInUser: Auth.loggedInUser,
        onlogin: function (assertion) {
            tryRequest({
                type: 'POST',
                url: Auth.loginUrl,
                data: { assertion: assertion },
                success: function (res, status, xhr) {
                    Auth.loggedInUser = xhr.responseText;
                    Auth.success('login');
                },
                error: function (xhr, status, err) {
                    navigator.id.logout();
                    Auth.error(err, 'login');
                }
            }, true);
        },
        onlogout: function () {
            tryRequest({
                type: 'POST',
                url: Auth.logoutUrl,
                data: {},
                success: function (res, status, xhr) {
                    Auth.loggedInUser = null;
                    Auth.success('logout');
                },
                error: function (xhr, status, err) {
                    Auth.error(err, 'logout');
                }
            }, true);
        }
    });
})(jQuery);
