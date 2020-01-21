import { createSnackbar } from "@snackbar/core";
import pull from "lodash/pull";
import { Component, h } from "preact";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { Library } from "../../utils/models";
import DirectoryBrowser from "../DirectoryBrowser";
import Loading from "../Loading";

interface Props {}
interface State {
    libraries: Library[]
    showDirectories: boolean
}

export default class LibraryManagement extends Component<Props, State> {
    public componentDidMount() {
        http.getJson('/api/library/list').then((libraries:Library[]) => this.setState({ libraries }));
    }
    public render(_, { libraries, showDirectories }) {
        return (
            <div>
                <form onSubmit={this.submitNewLibrary}>
                    <table class="fullwidth">
                        <thead>
                            <tr>
                                <td>{translate('name')}</td>
                                <td>{translate('kind')}</td>
                                <td>{translate('language')}</td>
                                <td>{translate('path')}</td>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                !libraries ? (<Loading />) : libraries.map(lib => (
                                    <tr key={lib.id}>
                                        <td>{lib.name}</td>
                                        <td>{translate(lib.kind)}</td>
                                        <td>{lib.language}</td>
                                        <td>{lib.path || translate('not specified')}</td>
                                        <td>
                                            <button type="button" onClick={this.indexLibrary(lib)} class="material-icons bordered">refresh</button>
                                            <button type="button" onClick={this.deleteLibrary(lib)} class="material-icons bordered">close</button>
                                        </td>
                                    </tr>
                                ))
                            }
                            <tr>
                                <td>
                                    <input class="inline-edit fullwidth" name="name" placeholder={translate('name')} type="text" required/>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="kind" required>
                                        <option value={'film'}>{translate('film')}</option>
                                        <option value={'series'}>{translate('series')}</option>
                                    </select>
                                </td>
                                <td>
                                    <select class="inline-edit fullwidth" name="language" required>
                                        <option value="en-UK">en-UK</option>
                                        <option value="da-DK">da-DK</option>
                                    </select>
                                </td>
                                <td>
                                    <input class="inline-edit fullwidth" name="path" placeholder={translate('path')} type="text" required/>
                                </td>
                                <td>
                                    <button class="material-icons bordered">done</button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </form>
                <button class="bordered" onClick={this.indexAllLibraries}>{translate('index-all-libraries')}</button>
                <button class="bordered" onClick={() => this.setState({ showDirectories: !showDirectories})}>{translate(showDirectories ? 'hide-directories' : 'show-directories')}</button>
                {showDirectories && (<DirectoryBrowser/>)}
            </div>
        );
    }


    private indexLibrary = (library:Library) => () => {
        http.post('/api/indexing', { value: [ library.id ] }).then(response => {
            if (response.ok) {
                createSnackbar(`Indexing ${library.name}..`, { timeout: 1500 });
            } else {
                createSnackbar('Unable to start indexing :/', { timeout: 1500 });
            }
        });
    };
    private deleteLibrary = (library:Library) => () => {
        http.delete('/api/library', { value: library.id }).then(response => {
            if (response.ok) {
                this.setState(s => { pull(s.libraries, library) });
                createSnackbar(`${library.name} deleted!`, { timeout: 1500 });
            } else {
                createSnackbar('Unable to remove the library :/', { timeout: 1500 });
            }
        });
    };
    private indexAllLibraries = () => {
        http.post('/api/indexing', { value: this.state.libraries.map(l => l.id) }).then(response => {
            if (response.ok) {
                createSnackbar(`Indexing all libraries..`, { timeout: 1500 });
            } else {
                createSnackbar('Unable to start indexing :/', { timeout: 1500 });
            }
        });
    };
    private submitNewLibrary = async (ev:Event) => {
        ev.preventDefault();
        const formElement = ev.target as HTMLFormElement;
        const form = new FormData(formElement);
        const response = await http.post('/api/library', form, false);
        if (response.ok) {
            createSnackbar('Library added!', { timeout: 1500 });
            const library:Library = await response.json();
            this.setState(s => { s.libraries.push(library) });
            formElement.reset();
        }
        else {
            createSnackbar('Unable to create new library :/', { timeout: 1500 });
        }
    };
}