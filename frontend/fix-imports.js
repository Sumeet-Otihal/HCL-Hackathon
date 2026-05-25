const fs = require('fs');
const path = require('path');

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = path.join(dir, file);
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(file));
        } else {
            if (file.endsWith('.ts')) results.push(file);
        }
    });
    return results;
}

const files = walk('src');
files.forEach(f => {
    let content = fs.readFileSync(f, 'utf8');
    let changed = false;

    const replaces = [
        { from: /..\/..\/..\/..\/environments/g, to: '../../environments' },
        { from: /..\/..\/..\/environments/g, to: '../../environments' },
        { from: /..\/..\/services/g, to: '../services' },
        { from: /..\/..\/models/g, to: '../models' },
        { from: /..\/..\/components/g, to: '../components' },
        { from: /..\/..\/utils/g, to: '../utils' }
    ];

    // Some specific fixes:
    // services are in src/app/services, environments is in src/environments
    // from src/app/services/auth.service.ts to environments -> `../../../environments` -> should be `../../environments` (wait: src/app/services -> ../.. -> src -> environments) => `../../environments/environment`

    // Let's refine based on the file depth.
    // src/app/pages/login.component.ts (depth 3 from root)
    // src/app/pages -> .. is app -> .. is src
    // So to models: `../models`
    // To components: `../components`
    // To services: `../services`
    // To environments: `../../environments/environment`

    replaces.forEach(r => {
        if (content.match(r.from)) {
            content = content.replace(r.from, r.to);
            changed = true;
        }
    });

    if (changed) {
        fs.writeFileSync(f, content);
        console.log(`Updated ${f}`);
    }
});
