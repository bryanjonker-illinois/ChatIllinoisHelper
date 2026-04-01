function blazorMenu() {
    document.querySelector('ilw-header').removeAttribute('compact');
    return true;
}

function copyToClipboard() {
    navigator.clipboard.writeText(document.getElementById('code-text').innerText).catch(function (error) { alert(error); });
    document.getElementById('code-button').innerText = 'Copied!';
    return true;
}