// Client-side adapters for the custom server-side validation attributes
// (GuardPost.Web.ViewModels.Validation.DateAfterAttribute / NotInFutureAttribute).
(function ($) {
    'use strict';

    if (!$ || !$.validator || !$.validator.unobtrusive) {
        return;
    }

    function parseDate(value) {
        var date = new Date(value);
        return isNaN(date.getTime()) ? null : date;
    }

    // data-val-dateafter="message" data-val-dateafter-other="StartTime"
    $.validator.addMethod('dateafter', function (value, element, params) {
        if (!value) {
            return true;
        }

        var otherValue = $(element).closest('form').find('[name="' + params.other + '"]').val();
        var current = parseDate(value);
        var other = parseDate(otherValue);

        return !current || !other || current > other;
    });

    $.validator.unobtrusive.adapters.add('dateafter', ['other'], function (options) {
        options.rules.dateafter = { other: options.params.other };
        options.messages.dateafter = options.message;
    });

    // data-val-notinfuture="message"
    $.validator.addMethod('notinfuture', function (value) {
        if (!value) {
            return true;
        }

        var date = parseDate(value);
        var tolerance = 5 * 60 * 1000;
        return !date || date.getTime() <= Date.now() + tolerance;
    });

    $.validator.unobtrusive.adapters.addBool('notinfuture');

    // Re-check the end of a shift when its start changes.
    $(document).on('change', '[name="StartTime"]', function () {
        var end = $(this).closest('form').find('[name="EndTime"]');
        if (end.length && end.val()) {
            end.valid();
        }
    });
})(window.jQuery);
