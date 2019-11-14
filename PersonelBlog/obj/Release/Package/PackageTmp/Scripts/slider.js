$(function () {

    var slider = $('.slider'),
		list = slider.find('ul.slider_liste'),
		length = list.find('li').length,
		width = slider.outerWidth(),
		totalWidth = width * length,
		index = 0,
		next = $('a.sonraki', slider),
		prev = $('a.onceki', slider);

    list.find('li').width(width).end().width(totalWidth);

    /* responsive */
    $(window).resize(function () {
        width = slider.outerWidth();
        totalWidth = width * length;
        list.find('li').width(width).end().width(totalWidth).css('margin-left', '-' + (index * width + 50) + 'px');
    });

    /* next */
    if (length > 1) {
        next.css('visibility', 'visible');
        prev.css('visibility', 'visible');
    }
    next.on('click', function () {
        if (index < length - 1) index++;
        list.stop().animate({
            marginLeft: '-' + (index * width)
        }, 500);
        return false
    });

    /* prev */
    prev.on('click', function () {
        if (index > 0) index--;
        list.stop().animate({
            marginLeft: '-' + (index * width)
        }, 500);
        return false
    });

    $.autoSlider = function () {

        if (index < length - 1) index++;

        else index = 0;

        list.stop().animate({

            marginLeft: '-' + (index * width)

        }, 500);

    }

    var interval = setInterval('$.autoSlider()', 5000);

    slider.hover(function () {

        clearInterval(interval);

        interval = null;

    }, function () {

        interval = setInterval('$.autoSlider()', 5000);

    });
});