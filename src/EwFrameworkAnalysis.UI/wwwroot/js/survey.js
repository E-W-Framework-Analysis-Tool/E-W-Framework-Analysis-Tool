window.SurveyHelpers = {
  readMultipleFilesAsText: (input) => {
    return Promise.all([...input.files].map(f => f.text()));
  },
  getFileNames: (input) => {
    return Promise.resolve([...input.files].map(f => f.name));
  }
};

window.BlazorHelpers = { clickElement: el => el.click(), clearFileInput: el => el.value = null }
