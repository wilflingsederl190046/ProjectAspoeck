const dateElement = document.getElementById('date');
const options = {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    timeZone: 'Europe/Berlin',
};
const formatter = new Intl.DateTimeFormat('de-DE', options);

function updateDate() {
    dateElement.textContent = formatter.format(new Date());
}

let timer = setInterval(updateDate, 1000);