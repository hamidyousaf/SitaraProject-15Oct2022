// Regex code adapted from:
// http://stackoverflow.com/questions/2632359/can-jquery-add-commas-while-user-typing-numbers

// Create the jQuery plugin
(function ($) {
    $.fn.commaTextbox = function () {
        var applyFormatting = function (that) {
            // Capture cursor position so we can restore it later
            var caretPosition = that.selectionStart
            //$('#selectionStart').text(selStart); // Temporary

            // Get the value from the textbox
            var origVal = $(that).val();
            //var originalSize = origVal.length;
            $('#origVal').text(origVal); // Temporary
            //$('#originalSize').text(originalSize); // Temporary

            // Get rid of commas and any other bad input
            var justNumbers = origVal.replace(/[^1234567890\.]/g, "");

            // Store the non-formatted number as a data attribute
            $(that).attr('data-raw-value', justNumbers);
            $('#justNumbers').text(justNumbers); // Temporary

            // If there are no numbers entered, blank out the box
            if (justNumbers.length == 0) {
                $(that).val('');
                return;
            }

            // Get rid of the decimal place and capture separately
            var decimalRegex = /(\d{0,})(\.(\d{1,})?)?/g
            var decimalPartMatches = decimalRegex.exec(justNumbers);
            var decimalPart = "";
            if (decimalPartMatches[2]) {
                decimalPart = decimalPartMatches[2];
            }
            $('#decimalPart').text(decimalPart); // Temporary
            var withoutDecimal = decimalPartMatches[1];
            $('#withoutDecimal').text(withoutDecimal); // Temporary

            // Assemble the final formatted value and put it in
            var final = '';
            //final += '$' // Now including this via CSS magic to avoid mucking with the form value
            final += withoutDecimal.replace(/\B(?=(\d{3})+(?!\d))/g, ",")
            final += decimalPart;
            $(that).val(final);
            $('#final').text(final); // Temporary

            // Figure out new caret position and restore it
            var origSelOffset = origVal.length - justNumbers.length;
            var selPosInNumber = caretPosition - origSelOffset;
            var newSelOffset = final.length - justNumbers.length;
            var newSelPos = selPosInNumber + newSelOffset;
            that.setSelectionRange(newSelPos, newSelPos);

            // TODO: Remove temporary debugging out
            $('#caretPosition').text(caretPosition);
            $('#origSelOffset').text(origSelOffset);
            $('#selPosInNumber').text(selPosInNumber);
            $('#newSelOffset').text(newSelOffset);
            $('#newSelPos').text(newSelPos);
        };

        // Format the current values
        this.each(function () {
            applyFormatting(this);
        });

        // Reapply formatting upon new input
        // Uses event namespacing so it can cleanly be removed later
        $(this).on('input.commaTextbox', function (event) {
            applyFormatting(this);
        });

        // Add some markup in order to position a currency symbol within the input
        // without affecting the value submitted to the server
       // this.addClass('commaTextbox');
       // this.wrap("<div class='commaTextbox-container'></div>");
       // this.parent().prepend("<div class='commaTextbox-currency'></div>");

        return this; // Allow for chaining of jQuery calls
    };
}(jQuery));