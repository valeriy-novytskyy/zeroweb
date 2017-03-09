/*--------------------------------------------------------*\
|   ______    __   |
|  |  __  |  |  |  |
|  | |  | |  |  |  |
|  | !__! |  |  |  |
|  !______!  !__!  |  binary : tech art
|
|  @file Directive that adds document style only on tab.
|  @requires ../angular/angular.js
|  @requires ../app.js
|----------------------------------------------------------
|  @author Valeriy Novytskyy
\*---------------------------------------------------------*/

'use strict';
    
/**
 * Register focus accessibility directive.
 */
angular.module('zeroApp')
    .directive('focusAccessibility', focusAccessibilityDirective);

/**
 * Implement focus accessibility directive.
 */
function focusAccessibilityDirective() {
    var toggleClass = 'show-focus-outlines';
    return {
        restrict: 'A',
        replace: false,
        link: function($scope, $element, attributes) {

            $element.keydown(function(e) {
                if (e.key === 'Tab') {
                    console.log('stuff happenend');
                    $('body').addClass(toggleClass);
                }
            });

            $element.click(function() {
                $('body').removeClass(toggleClass);
            });
        }
    };
}

/*
 * Get or create tooltip.
 * @returns {object} The tooltip element jQuery.
 */
function getTooltip() {
    var $tooltip = $('#tip');

    if ($tooltip.length === 0) {
        return createTooltip();
    }

    return $tooltip;
}

/*
 * Create a tooltip.
 * @returns {object} The new tooltip element jQuery.
 */
function createTooltip() {
    var div = '<div></div>';

    var ret = $(div)
        .attr('id', 'tip')
        .addClass('tooltip')
        .hide()
        .appendTo($('body'));

    ret.append($('<span></span>').addClass('tooltip-content'));
    ret.append($(div).addClass('tooltip-point'))

    return ret;
}

/*
 * Show or hide the tooltip for an element.
 * @param {bool} show - Whether to show or hide the tooltip.
 * @param {object} $element - Element for which to show the tooltip.
 */
function showTooltip(show, $element) {
    var $tooltip = getTooltip();

    if (show) {
        $tooltip
            .find('.tooltip-content')
            .text($element.attr('data-tooltip'));

        var y = Math.max(0,
            $element.offset().top -
            $tooltip.height() -
            14 -
            parseInt($element.attr('data-tooltip-offset-y') || 0));

        var x = Math.max(0,
            $element.offset().left +
            $element.width() / 2 -
            $tooltip.width() / 2 -
            parseInt($tooltip.css('padding-left')) -
            parseInt($element.attr('data-tooltip-offset-x') || 0));

        $tooltip.css({ left: x, top: y }).stop().fadeIn();
    } else {
        $tooltip.stop().fadeOut();
    }
}