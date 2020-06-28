function openHouse(evt, number) {
    document.getElementById("content").style.display = "block";
    var info = document.getElementById("info");
    info.rows[0].cells[1].innerText = data[number].centralHeatingRate;
    info.rows[1].cells[1].innerText = data[number].customHeatingRate;
    info.rows[2].cells[1].innerText = data[number].waterRate;
    info.rows[3].cells[1].innerText = data[number].generalWerRate;
    info.rows[4].cells[1].innerText = data[number].specialWerRate;

    var tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    evt.currentTarget.className += " active";
}