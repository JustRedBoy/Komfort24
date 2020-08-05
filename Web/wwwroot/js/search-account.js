const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/account")
    .build();

var waterRate;
var heatingRate;
var inputWaterValue = document.getElementById("waterValue");
var inputHeatingValue = document.getElementById("heatingValue");
var values = document.getElementById("values");

hubConnection.on("ReceiveAccountInfo", function (accountInfo) {
    if (accountInfo == null) {
        document.getElementById("account-not-found").style.display = "block";
        return;
    }
    document.getElementById("account-not-found").style.display = "none";
    document.getElementById("account-id").blur();
    document.getElementsByClassName("search")[0].classList.add("found");
    document.getElementById("account").style.display = "block";

    waterRate = accountInfo.waterRate;
    heatingRate = accountInfo.heatingRate;

    values.rows[1].cells[1].innerText = accountInfo.werStateStart + " грн";
    values.rows[2].cells[1].innerText = accountInfo.werPayment + " грн";
    values.rows[3].cells[1].innerText =
        (Math.round((accountInfo.werStateStart + accountInfo.werForMonth - accountInfo.werPayment) * 100) / 100) + " грн";

    values.rows[5].cells[1].innerText = accountInfo.waterCurrentValue;
    values.rows[6].cells[1].innerText = accountInfo.waterPayment + " грн";
    inputWaterValue.value = accountInfo.waterCurrentValue;

    values.rows[9].cells[1].innerText = accountInfo.heatingStateStart + " грн";
    if (accountInfo.heatingRate == "0") {
        document.getElementById("prev").style.display = "none";
        document.getElementById("cur").style.display = "none";
        values.rows[10].cells[1].innerText = accountInfo.heatingPayment + " грн";
        values.rows[11].cells[1].innerText = "-";
        inputHeatingValue.value = "";
        inputHeatingValue.disabled = true;
        values.rows[13].cells[1].innerText =
            (Math.round((accountInfo.heatingStateStart + accountInfo.heatingForMonth - accountInfo.heatingPayment) * 100) / 100) + " грн";
    }
    else {
        document.getElementById("prev").style.display = "table-row";
        document.getElementById("cur").style.display = "table-row";
        values.rows[10].cells[1].innerText = accountInfo.heatingPayment + " грн";
        values.rows[11].cells[1].innerText = accountInfo.heatingCurrentValue;
        inputHeatingValue.value = accountInfo.heatingCurrentValue;
        inputHeatingValue.disabled = false;
        values.rows[13].cells[1].innerText = accountInfo.heatingStateStart + " грн";
    }

    updateTotal();
});

//refactor
function updateTotal() {
    if (inputWaterValue.checkValidity() && (heatingRate == 0 || (heatingRate != 0 && inputHeatingValue.checkValidity()))) {
        var total = 0;
        var forWer = parseFloat(values.rows[3].cells[1].innerText);
        if (forWer > 0) {
            total += forWer;
        }

        var prevWaterValue = parseFloat(values.rows[5].cells[1].innerText);
        var waterValue = inputWaterValue.value - prevWaterValue;
        if (waterValue > 0) {
            total += waterValue * waterRate;
        }

        var forHeating = 0;
        if (heatingRate == 0) {
            forHeating = parseFloat(values.rows[13].cells[1].innerText);
            if (forHeating > 0) {
                total += forHeating;
            }
        }
        else {
            var heatingStateStart = parseFloat(values.rows[9].cells[1].innerText);
            var prevHeatingValue = parseFloat(values.rows[11].cells[1].innerText);
            var heatingValue = inputHeatingValue.value - prevHeatingValue;
            if (heatingValue > 0) {
                forHeating = (Math.round((heatingValue * heatingRate + heatingStateStart) * 100) / 100);
                values.rows[13].cells[1].innerText = forHeating + " грн";
                if (forHeating > 0) {
                    total += forHeating;
                }
            }
            else if (heatingStateStart > 0) {
                total += heatingStateStart;
            }
        }

        document.getElementById("total").innerText = "Итого к оплате: " + (Math.round(total * 100) / 100) + " грн";
    }
    else {
        if (heatingRate != 0) {
            values.rows[13].cells[1].innerText = values.rows[10].cells[1].innerText;
        }
        document.getElementById("total").innerText = "Ошибка при вводе!";
    }
}

function inputAccountId(accountId) {
    if (accountId.match(/(^\d{4}$)|(^\d{4}\/[1|2]$)/)) {
        hubConnection.invoke("GetAccountInfo", accountId);
    } else {
        document.getElementById("account-id").focus();
        document.getElementById("account").style.display = "none";
        document.getElementById("account-not-found").style.display = "none";
    }
}

hubConnection.start().then(() => inputAccountId(document.getElementById("account-id").value));