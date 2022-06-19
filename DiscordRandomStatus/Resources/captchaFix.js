var elementsToHide = document.querySelectorAll('body > div:not(.sample-form), body > br, legend, label, input');
for (var i = 0; i < elementsToHide.length; i++) {
    elementsToHide[i].style.display = 'none';
}

var centerElement = document.getElementsByClassName('sample-form')[0];
centerElement.style.margin = '0 auto';

var borderToHide = document.getElementsByTagName('fieldset')[0];
borderToHide.style.padding = 0;
borderToHide.style.border = 0;

document.body.style.background = '#36393F'

var successElement = document.querySelector('.hcaptcha-success');
var observation = new MutationObserver(
    function (mutationsList, observer) {
        if (mutationsList[0].addedNodes[0].data == 'Challenge Success!') {
            window.external.SolveCaptcha(hcaptcha.getResponse());
        }
    });
observation.observe(successElement, { characterData: false, childList: true, attributes: false });