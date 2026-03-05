document.addEventListener("DOMContentLoaded", function() {
    const popupOverlay = document.getElementById("welcome-popup-2");

    // Function to set a cookie
    function setCookie(name, value, minutes) {
        let expires = "";
        if (minutes) {
            const date = new Date();
            date.setTime(date.getTime() + (minutes * 60 * 1000));
            expires = "; expires=" + date.toUTCString();
        }
        document.cookie = name + "=" + (value || "")  + expires + "; path=/";
    }

    // Function to get a cookie
    function getCookie(name) {
        const nameEQ = name + "=";
        const ca = document.cookie.split(';');
        for(let i=0;i < ca.length;i++) {
            let c = ca[i];
            while (c.charAt(0)==' ') c = c.substring(1,c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
        }
        return null;
    }

    if (popupOverlay) {
        const continueToWebsiteButton = popupOverlay.querySelector("#continue-to-website-2");
        const contactFromPopupButton = popupOverlay.querySelector("#contact-from-popup-2");

        // Check if the cookie exists
        if (!getCookie("popupClosed")) {
            // Show the popup
            popupOverlay.style.display = "flex";
        }

        if (continueToWebsiteButton) {
            // Hide the popup and set the cookie when the "Continue to Website" button is clicked
            continueToWebsiteButton.addEventListener("click", function(event) {
                event.preventDefault();
                popupOverlay.style.display = "none";
                setCookie("popupClosed", "true", 1); // Set cookie to expire in 1 minute
            });
        }

        if (contactFromPopupButton) {
            contactFromPopupButton.addEventListener("click", function() {
                popupOverlay.style.display = "none";
                setCookie("popupClosed", "true", 1); // Set cookie to expire in 1 minute
            });
        }
    }
});