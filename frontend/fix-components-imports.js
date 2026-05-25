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

const files = walk('src/app/components');
files.forEach(f => {
    let content = fs.readFileSync(f, 'utf8');
    let changed = false;

    // fix the paths in components!
    if (content.includes("'../models'")) {
        content = content.replace(/'\.\.\/models'/g, "'../../models'");
        changed = true;
    }
    if (content.includes("'../services/auth.service'")) {
        content = content.replace(/'\.\.\/services\/auth\.service'/g, "'../../services/auth.service'");
        changed = true;
    }

    if (changed) {
        fs.writeFileSync(f, content);
        console.log(`Updated ${f}`);
    }
});
