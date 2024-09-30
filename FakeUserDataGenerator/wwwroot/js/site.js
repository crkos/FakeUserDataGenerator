// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let currentPage = 1;
const count = 20;

function reloadData() {
    const errors = $('#errorCount').val() || 0;
    const seed = parseInt($('#seedInput').val()) || 0;
    const locale = $('#region').val();

    const data = {
        Count: count,
        Errors: parseFloat(errors),
        Seed: seed,
        Page: currentPage,
        Locale: locale
    }

    $.ajax({
        url: '/User/Generate',
        type: 'POST',
        data: JSON.stringify(data), 
        contentType: 'application/json',
        success: function (response) {
            const tbody = $('#userTable tbody');
            tbody.empty();
            response.forEach(user => {
                tbody.append(`<tr><td>${user.index}</td><td>${user.randomIdentifier}</td><td>${user.fullName}</td><td>${user.address}</td><td>${user.phone}</td></tr>`);
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: " + error);
        }
    });
}

function validateErrors() {
    const errors = $('#errorCount');

    if (errors.val() > 1000) {
        errors.val(1000)
    }
}

$(document).ready(function () {
    $('#errorSlider').on('input', function () {
        $('#errorCount').val(parseFloat($(this).val()).toFixed(1));
    });

    $('#errorCount').on('input', function () {
        $('#errorSlider').val(parseFloat($(this).val()).toFixed(1));
    });

    $('#generateBtn').click(reloadData);
    $('#errorCount').on("input",reloadData);
    $('#errorCount').on("input",reloadData);
    $('#errorCount').on("input", validateErrors);
    $('#errorSlider').change(reloadData);
    $('#seedInput').on("input", reloadData);
    $('#region').change(reloadData);

    reloadData();
    generateRandomSeed();
});

$(window).scroll(function () {
    if ($(window).scrollTop() + $(window).height() >= $(document).height()) {
        currentPage++;
        loadMoreUsers(currentPage);
    }
});

function loadMoreUsers(page) {
    const errors = $('#errorCount').val() || 0;
    const seed = parseInt($('#seedInput').val()) || 0;
    const locale = $('#region').val();

    const data = {
        Count: count,
        Errors: parseFloat(errors),
        Seed: seed,
        Page: currentPage,
        Locale: locale
    }

    $.ajax({
        url: '/User/Generate',
        type: 'POST',
        data: JSON.stringify(data), 
        contentType: 'application/json',  
        success: function (response) {
            const tbody = $('#userTable tbody');
            response.forEach(user => {
                tbody.append(`<tr><td>${user.index}</td><td>${user.randomIdentifier}</td><td>${user.fullName}</td><td>${user.address}</td><td>${user.phone}</td></tr>`);
            });
        },
        error: function (xhr, status, error) {
            console.error("Error: " + error);
        }
    });
}

function generateRandomSeed() {
    const seedInput = document.getElementById('seedInput');
    const randomNumbers = Array.from({ length: 7 }, () => Math.floor(Math.random() * 5).toString());
    seedInput.value = randomNumbers.join('');
    reloadData();
}