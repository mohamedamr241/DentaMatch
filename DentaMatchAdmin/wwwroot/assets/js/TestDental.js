
document.getElementById("getInfoForm").addEventListener("submit", function (event) {
    event.preventDefault();
    callReport();
})

async function callReport() {
    try {
        var userId = document.getElementById("submitButtonDoctor").getAttribute("value")
        const response = await fetch("/Test/DentalDiseases?userId=" + userId, {
            method: "POST"
        });
        if (!response.ok) {
            throw new Error(`Failed to do job`);
        }
        else {
            const newData = await response.json();
            console.log(newData)
            var tar = document.getElementById("target");
            tar.innerHTML = '';
            for (var i = 0; i < newData.diseases.length; i++) {
                var row = document.createElement("tr");
                row.innerHTML = `
            <td>${newData.diseases[i]}</td>
            <td>${newData.numOfCases[i]}</td>
        `;
                tar.appendChild(row);
            }
        }
    }
    catch (error) {
        console.log(error)
    }
}