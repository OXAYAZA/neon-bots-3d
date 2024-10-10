import server from "oxyz-express";

server({
    browserSync: {
        enable: true
    },
    watcher: {
        enable: true
    },
    pug: {
        enable: true,
        root: 'dev'
    },
    sass: {
        enable: true,
        root: 'dev'
    }
});
