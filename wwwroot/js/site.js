// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('.btn-product-import').click(function () {
        var count = $('#txt-product-count').val();
        if (!count) {
            $('.txt-product-count-error').show();
        }
        else {
            $('.txt-product-count-error').hide();
            $.get('/api/products/fakeimport/' + count, function () {
                location.reload();
            });
        }  
    });

    $('.btn-product-reindex').click(function () {
        $.get('/api/search/reindex', function () {
            location.reload();
        });
    });

    $('.btn-product-delete').click(function () {
        var that = this;
        $.ajax({
            url: '/api/products/' + $(that).data('id'),
            type: 'DELETE',
            success: function (result) {
                $(that).closest('tr').remove();
            }
        });
    });

    $('#txt-product-search').keyup(function () {
        if ($(this).val().length >= 2) {
            $.get('/api/search/find?query=' + $(this).val(), function (data) {
                $('.search-result').html('');
                $(data).each(function (index, element) {
                    var itemTpl = $($('#searchitem').html());
                    itemTpl.find('.product-name').html(element.name);
                    itemTpl.find('.product-description').html(element.description);
                    itemTpl.find('.product-category').html(element.category);
                    itemTpl.find('.product-brand').html(element.brand);
                    $('.search-result').append(itemTpl);
                })
                $('.search-result').show();
            });
        }
        else {
            $('.search-result').hide();
        }
    })
})
