function getRandomFileName() {
    let timestamp = new Date().toISOString().replace(/[-:.]/g,"");
    let random = ("" + Math.random()).substring(2, 8);
    return timestamp + random;
}

export function renameFile(originalFile) {
    const myArray = originalFile.name.split(".");
    let type = myArray[1];
    let newFileName = getRandomFileName() + "." + type;
    return new File([originalFile], newFileName, {
        type: originalFile.type,
        lastModified: originalFile.lastModified,
    });
}