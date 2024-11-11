#!/bin/bash
# BASE_URL="https://bambi11.enoflag.de"

if [ -z "${BASE_URL}" ]; then
    echo "BASE_URL is not set!"
fi

CHECKIN_API_URL="${BASE_URL}/api/admin/checkinteam"
BOOT_API_URL="${BASE_URL}/api/admin/bootvm"

IPS_API_URL="${BASE_URL}/api/data/ips"

if [ -z "${ADMIN_SECRET}" ]; then
    echo "ADMIN_SECRET is not set!"
fi


CheckInTeam () {
    if [[ "$#" -lt 1 ]]; then
        echo "CheckInTeam TEAM_ID";
        return -1;
    fi

    TEAM_ID="$1"
    curl --get "$CHECKIN_API_URL" \
        --data-urlencode "adminSecret=${ADMIN_SECRET}" \
        --data-urlencode "id=${TEAM_ID}"
}


BootVM () {
    if [[ "$#" -lt 1 ]]; then
        echo "BootVM TEAM_ID";
        return -1;
    fi

    TEAM_ID="$1"
    curl --get "$BOOT_API_URL" \
        --data-urlencode "adminSecret=${ADMIN_SECRET}" \
        --data-urlencode "teamId=${TEAM_ID}"
}

BootAllCheckins () {
    # while true; do 
    IP_LIST="$(curl "$IPS_API_URL")"
    echo "$IP_LIST"

    for ip in $IP_LIST; do
	TEAM_ID=`echo $ip | cut -d. -f 3`

	echo "ID $TEAM_ID ($ip)"
	BootVM "$TEAM_ID"

	sleep 2;
    done
}

