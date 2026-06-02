window.ProjectHeaderHelpers = {
  downloadFile: function (filename, content, contentType) {
    const blob = new Blob([content], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
  },

  triggerFileInput: function (element) {
    element.click();
  },

  // Reset file input value so that <input onchange="..."> is always changed even when trying to reload the same file
  resetFileInput: function (element) {
    element.value = null;
  },

  readFileAsText: function (element) {
    return new Promise((resolve) => {
      const file = element.files[0];
      if (!file) {
        resolve(null);
        return;
      }
      const reader = new FileReader();
      reader.onload = (e) => resolve(e.target.result);
      reader.readAsText(file);
    });
  }
};
