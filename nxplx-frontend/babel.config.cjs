module.exports = api => {
    api.cache(true);
    const presets = [
        ["preact-cli/babel", { "targets": { "commonjs": true } }]
    ];

    const env = process.env.NODE_ENV.trim();
    console.log(` ${env} build`);
    const plugins = env === "production" ? (
        [
            "babel-plugin-styled-components",
            ["@babel/plugin-proposal-private-methods", { "loose": true }],
            ["@babel/plugin-proposal-class-properties", { "loose": true }]
        ]
    ) : [

        "babel-plugin-styled-components",
        ["@babel/plugin-proposal-private-methods", { "loose": true }],
        ["@babel/plugin-proposal-class-properties", { "loose": true }]
    ];

    return {
        presets,
        plugins
    };
};