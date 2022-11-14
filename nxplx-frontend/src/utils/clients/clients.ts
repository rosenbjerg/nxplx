import {MyGroupingsClient, MyPlantClient, MyPlantNotesClient, MySubscriptionClient, MyUserClient, PlantClient} from "./api.generated";
import {AuthenticationClient, UserClient} from "./authentication.generated";
import {ImageClient} from "./images.generated";

export default class Clients {
    static plants = new PlantClient()
    static myPlants = new MyPlantClient()
    static myPlantNotes = new MyPlantNotesClient()
    static myGroupings = new MyGroupingsClient()
    static myUser = new MyUserClient()
    static mySubscription = new MySubscriptionClient()
    static authentication = new AuthenticationClient()
    static image = new ImageClient()
    static user = new UserClient()
}