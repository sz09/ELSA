import { NgForm } from "@angular/forms";

export function  markAllControlDirty(form: NgForm) {
  var keys = Object.keys(form.controls);
  keys.forEach(key => {
    form.controls[key].markAsDirty();
    form.controls[key].markAsTouched();
  })
}