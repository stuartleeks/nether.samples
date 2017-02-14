/// <binding AfterBuild='copy-packages' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');

gulp.task('default', function () {
    // place code for your default task here
});

gulp.task("copy-packages", function () {
    gulp.src([
        "oidc-client/dist/**"
    ], {
        cwd: "node_modules/**"
     })
    .pipe(gulp.dest("./wwwroot/lib"));
});