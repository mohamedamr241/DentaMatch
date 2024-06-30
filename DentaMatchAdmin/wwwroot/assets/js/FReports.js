document.getElementById("getInfoForm").addEventListener("submit", function (event) {
    event.preventDefault();
    callReports();
});

async function callReports() {
    try {
        var userId = document.getElementById("submitButtonDoctor").getAttribute("value");

        const request = await fetch("/Doctor/Reports?userId=" + userId, {
            method: "POST"
        });

        if (!request.ok) {
            throw new error("failed to contact with server")
        }
        else {
            const newData = await request.json();
            console.log(newData)
            var tar = document.getElementById("target");
            tar.innerHTML = '';
            for (var i = 0; i < newData.patients.length; i++) {
                var row = document.createElement("tr");
                row.innerHTML = `
                <td>${newData.patients[i]}</td>
                <td>${newData.doctors[i]}</td>
                <td>${newData.numOfReports[i]}</td>`;
                tar.appendChild(row);
            }
        }
    }
    catch (error) {
        console.log(error)
    }
}