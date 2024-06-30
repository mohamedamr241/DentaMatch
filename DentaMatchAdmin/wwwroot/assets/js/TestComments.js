document.getElementById("getInfoForm").addEventListener("submit", function (event) {
    event.preventDefault();
    CallComments();
})

async function CallComments() {
    try {
        var userId = document.getElementById("submitButtonDoctor").getAttribute("value")
        const request = await fetch("/Test/Comments?userId=" + userId, {
            method: "POST"
        });
        if (!request.ok) {
            throw new error("failed")
        }
        else {
            const newData = await request.json();
            console.log(newData)
            var tar = document.getElementById("target");
            tar.innerHTML = '';
            for (var i = 0; i < newData.cases.length; i++) {
                var row = document.createElement("tr");
                row.innerHTML = `
            <td>${newData.cases[i]}</td>
            <td>${newData.userNames[i]}</td>
            <td>${newData.numOfComments[i]}</td>
        `;
                tar.appendChild(row);
            }
        }
    }
    catch (error) {
        console.log(error)
    }
}