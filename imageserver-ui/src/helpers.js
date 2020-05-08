export function getFile () {
  return new Promise((resolve, reject) => {
    var input = document.createElement('input');
    input.type = 'file';
    input.accept = "image/jpeg";
    input.onerror = err => reject(err);
    input.onchange = () => {
      if (input.files.length === 1) {
        resolve(input.files[0]);
      } else {
        reject("no file selected");
      }
    };
    input.click();
  });
}