const dateElement = document.getElementById('date');
const options = {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: 'numeric',
    minute: 'numeric',
    second: 'numeric',
    timeZone: 'Europe/Berlin',
};
const formatter = new Intl.DateTimeFormat('de-DE', options);

const statusElement = document.getElementById('orderstate');
const btnPlaceOrder = document.getElementById('btnPlaceOrder');

function updateDate() {
    dateElement.textContent = formatter.format(new Date());
    if (new Date().getHours() >= 0 && new Date().getHours() <= 18) {
        dateElement.style.backgroundColor = '#50C878';

        statusElement.textContent = "Bestellung möglich";
        statusElement.style.backgroundColor = '#50C878';
        btnPlaceOrder.disabled = false;
    } else {
        dateElement.style.backgroundColor = 'red';
        statusElement.textContent = "Bestellzeitraum abgelaufen";
        statusElement.style.backgroundColor = 'red';
        btnPlaceOrder.disabled = true;
    }
}

let timer = setInterval(updateDate, 1000);

