import { createSnackbar } from "@snackbar/core";
import { Component, h } from "preact";
import { add, remove } from "../../utils/arrays";
import http from "../../utils/http";
import { translate } from "../../utils/localisation";
import { Library } from "../../utils/models";
import Loading from "../Loading";
import Flag from "../Flag";
import CreateLibraryModal from "../../modals/CreateLibraryModal";

interface Props {}
interface State {
    libraries: Library[]
    createLibraryModalOpen: boolean
}

export default class LibraryManagement extends Component<Props, State> {
    public componentDidMount() {
        http.getJson<Library[]>('/api/library/list').then(libraries => this.setState({ libraries }));
    }
    public render(_, { libraries, createLibraryModalOpen }) {
        return (
            <div>
                {createLibraryModalOpen && (
                    <CreateLibraryModal onDismiss={() => this.setState({createLibraryModalOpen: false})} onCreated={lib => this.setState({ libraries: add(this.state.libraries, lib) })} />
                )}

                <ul class="fullwidth" style="list-style-type: none; margin: 0; padding: 0;">
                    {
                        !libraries ? (<Loading />) : libraries.map(lib => (
                            <li key={lib.id} class="bordered" style="display: inline-block; padding: 2px 8px; margin: 2px">
                                <div style="display: inline-grid; grid-template-columns: 32px auto;">
                                    <div style="margin: auto 0" title={translate(lib.kind)}>
                                        <i class="material-icons" style="">{lib.kind === 'series' ? 'tv' : 'local_movies'}</i>
                                    </div>
                                    <div>
                                        <div>
                                            <b title={translate('title')}>{lib.name}</b>
                                            <Flag language={lib.language}/>
                                        </div>
                                        <button type="button" title={translate('index library')} onClick={this.indexLibrary(lib)} class="material-icons noborder" style="font-size: 18px;">refresh</button>
                                        <button type="button" title={translate('delete library')} onClick={this.deleteLibrary(lib)} class="material-icons noborder" style="font-size: 18px;">close</button>
                                    </div>
                                </div>
                            </li>
                        ))
                    }
                </ul>
                <button class="bordered" onClick={() => this.setState({ createLibraryModalOpen: true })}>{translate('create-library')}</button>
                <button class="bordered" onClick={this.indexAllLibraries}>{translate('index-all-libraries')}</button>
            </div>
        );
    }


    private indexLibrary = (library:Library) => async () => {
        const response = await http.post('/api/indexing', [ library.id ]);
        const msg = response.ok ? `Indexing ${library.name}..` : 'Unable to start indexing :/';
        createSnackbar(msg, { timeout: 1500 });
    };
    private deleteLibrary = (library:Library) => async () => {
        const response = await http.delete('/api/library', [ library.id ]);
        if (response.ok) this.setState({ libraries: remove(this.state.libraries, library) });
        const msg = response.ok ? `${library.name} deleted!` : 'Unable to remove the library :/';
        createSnackbar(msg, { timeout: 1500 });
    };
    private indexAllLibraries = async () => {
        const response = await http.post('/api/indexing', [ this.state.libraries.map(lib => lib.id) ]);
        const msg = response.ok ? 'Indexing all libraries..' : 'Unable to start indexing :/';
        createSnackbar(msg, { timeout: 1500 });
    };
}
