// GuardPost – small UI helpers (no framework needed).
(function () {
    'use strict';

    // Quick shift-length buttons on the shift form: set End = Start + N hours.
    document.querySelectorAll('[data-shift-hours]').forEach(function (button) {
        button.addEventListener('click', function () {
            var form = button.closest('form');
            var start = form.querySelector('[name="StartTime"]');
            var end = form.querySelector('[name="EndTime"]');
            if (!start || !end || !start.value) {
                return;
            }

            var date = new Date(start.value);
            date.setHours(date.getHours() + parseInt(button.dataset.shiftHours, 10));
            end.value = toLocalInputValue(date);
            end.dispatchEvent(new Event('change', { bubbles: true }));
        });
    });

    // License numbers are stored in upper case – help the user while typing.
    var license = document.querySelector('[name="LicenseNumber"]');
    if (license) {
        license.addEventListener('input', function () {
            var position = license.selectionStart;
            license.value = license.value.toUpperCase();
            license.setSelectionRange(position, position);
        });
    }

    function pad(n) {
        return n < 10 ? '0' + n : '' + n;
    }

    function toLocalInputValue(date) {
        return date.getFullYear() + '-' + pad(date.getMonth() + 1) + '-' + pad(date.getDate()) +
            'T' + pad(date.getHours()) + ':' + pad(date.getMinutes());
    }
})();
