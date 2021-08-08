module.exports = {
    env: {
        browser: true
    },
    extends: [
        "eslint:recommended",
        "plugin:react/recommended",
        "plugin:react-hooks/recommended"
    ],
    parserOptions: {
        ecmaFeatures: {
            jsx: true
        },
        project: "./tsconfig.eslint.json"
    },
    rules: {
        "react/no-unknown-property": ["error", { ignore: ["class"] }],
        "quotes": [2, "single", { "avoidEscape": true }]
    },
    settings: {
        react: {
            pragma: "h",
            version: "detect"
        }
    },
    overrides: [
        {
            files: ["*.js"],
            rules: {
                "@typescript-eslint/explicit-function-return-type": "off"
            }
        },
        {
            files: ["*.tsx", "*.jsx"],
            rules: {
                "@typescript-eslint/no-unsafe-member-access": "off",
                "@typescript-eslint/no-inferrable-types": "off"
            }
        }
    ]
};
