function numberToWords(amount) {
    // Split the number into dollars and cents
    var dollars = Math.floor(amount);
    var cents = Math.round((amount - dollars) * 100);

    // Function to convert a number less than 1000 to words
    function convertLessThanOneThousand(number) {
        var ones = ['', 'ONE', 'TWO', 'THREE', 'FOUR', 'FIVE', 'SIX', 'SEVEN', 'EIGHT', 'NINE'];
        var teens = ['TEN', 'ELEVEN', 'TWELVE', 'THIRTEEN', 'FOURTEEN', 'FIFTEEN', 'SIXTEEN', 'SEVENTEEN', 'EIGHTEEN', 'NINETEEN'];
        var tens = ['', '', 'TWENTY', 'THIRTY', 'FORTY', 'FIFTY', 'SIXTY', 'SEVENTY', 'EIGHTY', 'NINETY'];

        var words = '';

        if (number >= 100) {
            words += ones[Math.floor(number / 100)] + ' HUNDRED ';
            number %= 100;
        }

        if (number >= 20) {
            words += tens[Math.floor(number / 10)] + ' ';
            number %= 10;
        }

        if (number > 0) {
            if (number < 10) {
                words += ones[number];
            } else {
                words += teens[number - 10];
            }
        }

        return words.trim();
    }

    // Function to convert dollars and cents to words
    function dollarsToWords(dollars, cents) {
        var words = '';

        // Convert dollars part
        if (dollars > 0) {
            var billions = Math.floor(dollars / 1000000000);
            var millions = Math.floor((dollars % 1000000000) / 1000000);
            var thousands = Math.floor((dollars % 1000000) / 1000);
            var hundreds = dollars % 1000;

            if (billions > 0) {
                words += convertLessThanOneThousand(billions) + ' BILLION ';
            }

            if (millions > 0) {
                words += convertLessThanOneThousand(millions) + ' MILLION ';
            }

            if (thousands > 0) {
                words += convertLessThanOneThousand(thousands) + ' THOUSAND ';
            }

            if (hundreds > 0) {
                words += convertLessThanOneThousand(hundreds) + ' ';
            }

            words;
        } else {
            words = 'PROVIDE NUMBER';
        }

        // Convert cents part
        if (cents > 0) {
            words += ' AND ' + convertLessThanOneThousand(cents);
        }

        return words.trim();
    }

    // Return the final words
    return dollarsToWords(dollars, cents);
}