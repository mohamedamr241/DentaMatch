document.getElementById("getInfoForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent the default action of following the link
    CallApi();
});
async function CallApi() {
    try {
        var id = document.getElementById("submitButtonDoctor").getAttribute("value");
        console.log(id)
        const response = await fetch('/Doctor/Comments?userId=' + id, {
            method: 'POST'
        });
        console.log(response)
        if (!response.ok) {
            throw new Error(`Failed to verify user`);
        }
        else {
            const newData = await response.json();
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

    } catch (error) {
        console.error('Error:', error);
        // Handle error here
    }
}