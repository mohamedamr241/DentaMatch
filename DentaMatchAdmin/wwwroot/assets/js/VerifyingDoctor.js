document.getElementById("VerifyUser").addEventListener("click", function (event) {
    event.preventDefault(); // Prevent the default action of following the link
    verifyUser(true);
});
document.getElementById("RejectUser").addEventListener("click", function (event) {
    event.preventDefault(); // Prevent the default action of following the link
    verifyUser(false);
});
async function verifyUser(Status) {
    try {
        var id = document.getElementById("VerifyUser").getAttribute("value"); 
        console.log(Status)
        const response = await fetch('/VerifyUser/Users?id=' + id + '&Status=' + Status, {
            method: 'POST'
        });
        console.log(response)
        if (!response.ok) {
            throw new Error('Failed to verify user');
        }
        else {
            window.location.reload();
        }
    } catch (error) {
        console.error('Error:', error);
        // Handle error here
    }
}