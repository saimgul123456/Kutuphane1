var numberElement = document.querySelector('.number-decimal');
numberElement.addEventListener('change', alterNumber);

function alterNumber(event) {
    var el = event.target;
    var elValue = el.value;
    el.value = parseFloat(elValue).toFixed(2);
}