# FinnStatistikk Notes


We are interested in data/insights like:
- Agencies and tracking things back to them (eg. what sort of ads do they publish, do the ads typically contain X or Y, where do they typically publish, how many visits are they doing and at which interval, how successful they are and where, etc.)
- Information about the sale process (eg. AcquisitionDTO, ElectronicBidDto, Preemption, etc.)
- Information about regulations,
- How long does it for an apartment to be leased or a property/plot to be sold depending on its characteristics (location, price, size, amneties, etc.)
- How do rental/property/plot prices correlate to other attributes (location, size, amneties, etc.)
- Location-dependant information such as price per square meter in a given area, turnover rate per region, mean time to rent or sell per region, or where can you find the most farms/småbruk, or where are rental/property/plot prices the lowest/highest, the number of ads posted by agencies vs. by private users per area, etc.
- Ability to predict how likely it is lease a certain type of property (eg. with defined parameters such as geolocation, price, size, number of rooms, energy label, amneties, etc.), how long it would take, the typical min/max price, etc.
- The "success" rate or time to rental/sale depending on the ads and product attributes (eg. number of photos, price, location, size, number of rooms, plot size (eg. 10 hectares), etc.)
- Trends for these metrics (eg. evolution of prices, number of ads, globally and per region, etc.)
- Common keywords and how do descriptions correlate with "success" and possibly other metrics/data
- Pricing and advertisement strategies (eg. how ads evolve over time until they are "successful" (aka. sold/leased/etc.))
- How long do development projects typically last (eg. from when the ad is initially posted, to the claimed acquisition date)
- What type of developments are popular and where, what are the property characteristics, what scale of development (eg. small/large allotment, condomium, high rise, etc.), what companies are involved, etc.
- When an advertisement for a leased properties appears, we need to check if we have already have it in record and track its history. This will help us understand what type of leased properties are problematic (eg. high turnover rate), and potentially what agencies are associated with such problematic ads.
- We also have a drone business that offers data capture and analytics for GIS, visuals, marketing, progress monitoring and reports, etc. It would be useful for us to have information about prospect clients (eg. developers, construction companies, architects, agencies, etc.), and future development projects.
- We could use a multi-modal AI like Gemini 2.5 Flash to infer additional details from photos and text. We could submit the ad's data (eg. photos, description, price, location, etc.) and send it an empty json structure as a sort of "questionnaire" where it would fill in the empty value fields for each property in the json structure. We could dynamically add or remove fields to the questionnaire depending on which information is available/missing in the ad, as well as have "permanent" fields that are always in the questionnaire (eg. for data that are never available straight out of Finn). For example, if the number of levels/floors is missing the AI could try to determine the value from pictures. We could also ask it to only answer if it is highly confident in its answer (eg. p>=0.95)
- We need to train AI/prediction engines in order to predict things like: how likely it is lease a certain type of property (eg. with defined parameters such as geolocation, price, size, number of rooms, energy label, amneties, etc.), how long it would take, the typical min/max price, etc.
